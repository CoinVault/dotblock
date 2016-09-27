// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bip39.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   BIP39 implementation of Mnemonic wallet backup
//   Credit goes to https://github.com/Thashiznets/BIP39.NET
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Mnemonic
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Mnemonic.Wordlists;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;

    #endregion

    /// <summary>
    /// BIP39 implementation of Mnemonic wallet backup
    /// Credit goes to https://github.com/Thashiznets/BIP39.NET
    /// </summary>
    public class Bip39
    {
        #region Private Attributes

        /// <summary>
        /// The pass code bytes.
        /// </summary>
        private byte[] passphraseBytes;

        /// <summary>
        /// The language.
        /// </summary>
        private Language language;

        /// <summary>
        /// The _word index list.
        /// </summary>
        private List<int> wordIndexList; // I made this a property because then we can keep the same index and swap between languages for experimenting

        /// <summary>
        /// The mnemonic sentence.
        /// </summary>
        private string mnemonicSentence;

        #endregion

        #region Public Constants and Enums

        /// <summary>
        /// The c minimum entropy bits.
        /// </summary>
        public const int CMinimumEntropyBits = 128;

        /// <summary>
        /// The c maximum entropy bits.
        /// </summary>
        public const int CMaximumEntropyBits = 8192;

        /// <summary>
        /// The c entropy multiple.
        /// </summary>
        public const int CEntropyMultiple = 32;

        /// <summary>
        /// The c bits in byte.
        /// </summary>
        public const int CBitsInByte = 8;

        /// <summary>
        /// The c bit group size.
        /// </summary>
        public const int CBitGroupSize = 11;

        /// <summary>
        /// The c empty string.
        /// </summary>
        public const string CEmptyString = "";

        /// <summary>
        /// The c salt header.
        /// </summary>
        public const string CSaltHeader = "mnemonic"; // this is the first part of the salt as described in the BIP39 spec

        /// <summary>
        /// The language.
        /// </summary>
        public enum Language
        {
            /// <summary>
            /// The english.
            /// </summary>
            English, 

            /// <summary>
            /// The japanese.
            /// </summary>
            Japanese, 

            /// <summary>
            /// The spanish.
            /// </summary>
            Spanish, 

            /// <summary>
            /// The chinese simplified.
            /// </summary>
            ChineseSimplified, 

            /// <summary>
            /// The chinese traditional.
            /// </summary>
            ChineseTraditional, 

            /// <summary>
            /// The French.
            /// </summary>
            French, 

            /// <summary>
            /// The unknown.
            /// </summary>
            Unknown
        };

        /// <summary>
        /// The c jp space string.
        /// </summary>
        public const string CJpSpaceString = "\u3000"; // ideographic space used by japanese language

        #endregion

        #region Constructors

        /////// <summary>
        /////// Constructor to build a BIP39 object from scratch given an entropy size and an optional passphrase. Language is optional and will default to English
        /////// </summary>
        /////// <param name="entropySize">The size in bits of the entropy to be created</param>
        /////// <param name="passphrase">The optional passphrase. Please ensure NFKD Normalized, Empty string will be used if not provided as per spec</param>
        /////// <param name="language">The optional language. If no language is provided English will be used</param>
        ////public BIP39(int entropySize=cMinimumEntropyBits, string passphrase=cEmptyString, Language language=Language.English)
        ////{
        ////    //check that ENT size is a multiple of 32 and at least minimun entropy size to stop silly people using tiny entropy, oh also making sure entropy size doesn't exceed our checksum bits available
        ////    if (entropySize % cEntropyMultiple != 0 || entropySize < cMinimumEntropyBits || entropySize > cMaximumEntropyBits)
        ////    {
        ////        throw (new Exception("entropy size must be a multiple of "+cEntropyMultiple+" (divisible by "+cEntropyMultiple+" with no remainder) and must be greater than " + (cMinimumEntropyBits-1) + " and less than "+(cMaximumEntropyBits+1)));
        ////    }

        ////    this.entropyBytes = Utilities.GetRandomBytes(entropySize / cBitsInByte); //crypto random entropy of the specified size
        ////    Init(passphrase, language);
        ////}

        /// <summary>
        /// Initializes a new instance of the <see cref="Bip39"/> class. 
        /// Constructor to build a BIP39 object using supplied entropy bytes either from a previously created BIP39 object or another method of entropy generation.
        /// </summary>
        /// <param name="entropyBytes">
        /// The bytes.
        /// </param>
        /// <param name="passphrase">
        /// The entropy bytes which will determine the mnemonic sentence
        /// </param>
        /// <param name="language">
        /// The optional language. If no language is provided English will be used
        /// </param>
        public Bip39(byte[] entropyBytes, string passphrase = CEmptyString, Language language = Language.English)
        {
            // check to ensure at least 16 bytes no more than 1024 bytes and byte array is in 4 byte groups
            if ((entropyBytes.Length * CBitsInByte) % CEntropyMultiple != 0 || (entropyBytes.Length * CBitsInByte) < CMinimumEntropyBits)
            {
                throw new MnemonicException("entropy bytes must be a multiple of " + (CEntropyMultiple / CBitsInByte) + " (divisible by " + (CEntropyMultiple / CBitsInByte) + " with no remainder) and must be greater than " + ((CMinimumEntropyBits / CBitsInByte) - 1) + " bytes and less than " + ((CMaximumEntropyBits / CBitsInByte) + 1) + " bytes");
            }

            this.EntropyBytes = entropyBytes;
            this.Init(passphrase, language);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bip39"/> class. 
        /// Constructor to build a BIP39 object using a supplied Mnemonic sentence and pass phrase. If you are not worried about saving the entropy bytes, or using custom words not in a wordlist, you should consider the static method to do this instead.
        /// </summary>
        /// <param name="mnemonicSentence">
        /// The sentence.
        /// </param>
        /// <param name="passphrase">
        /// The mnemonic sentences used to derive seed bytes, Please ensure NFKD Normalized
        /// </param>
        /// <param name="language">
        /// Optional language to use for wordlist, if not specified it will auto detect language and if it can't detect it will default to English
        /// </param>
        public Bip39(string mnemonicSentence, string passphrase = CEmptyString, Language language=Language.Unknown)
        {
            this.mnemonicSentence = mnemonicSentence.Normalize(NormalizationForm.FormKD).Trim(); // just making sure we don't have any leading or trailing spaces
            this.passphraseBytes = Encoding.UTF8.GetBytes(passphrase.Normalize());
            string[] words = this.mnemonicSentence.Split(' ');

            // no language specified try auto detect it
            if(language.Equals(Language.Unknown))
            {
                this.language = AutoDetectLanguageOfWords(words);

                if(this.language.Equals(Language.Unknown))
                {
                    // yeah.....have a bias to use English as default....
                    this.language = Language.English;
                }
            }

            // if the sentence is not at least 12 characters or cleanly divisible by 3, it is bad!
            if (words.Length < 12 || words.Length % 3 != 0)
            {
                throw new MnemonicException("Mnemonic sentence must be at least 12 words and it will increase by 3 words for each increment in entropy. Please ensure your sentence is at leas 12 words and has no remainder when word count is divided by 3");
            }

            this.language = language;
            this.wordIndexList = this.RebuildWordIndexes(words);
            this.EntropyBytes = this.ProcessIntToBitsThenBytes(this.wordIndexList);   
        }

        #endregion

        #region Public Static Methods

        /////// <summary>
        /////// An asynchronous static method to create a new BIP39 from random entropy. The random entropy creation is CPU intensive so is run in its own Task and we await as per async pattern.
        /////// </summary>
        /////// <param name="entropySize">The size in bits of the entropy to be created</param>
        /////// <param name="passphrase">The optional passphrase. Please ensure NFKD Normalized, Empty string will be used if not provided as per spec</param>
        /////// <param name="language">The optional language. If no language is provided English will be used</param>
        /////// <returns>A BIP39 object</returns>
        ////public static async Task<BIP39> GetBIP39Async(int entropySize = cMinimumEntropyBits, string passphrase = cEmptyString, Language language = Language.English)
        ////{
        ////    byte[] entropyBytes = await Utilities.GetRandomBytesAsync(entropySize / cBitsInByte);
        ////    return new BIP39(entropyBytes, passphrase, language);
        ////}

        /// <summary>
        /// Takes in a string[] of words and detects the language that has the highest number of matching words.
        /// </summary>
        /// <param name="words">
        /// The words of which you wish to derive a language
        /// </param>
        /// <returns>
        /// The best attempt at a guessed Language
        /// </returns>
        public static Language AutoDetectLanguageOfWords(string[] words)
        {
            English eng = new English();
            Japanese jp = new Japanese();
            Spanish es = new Spanish();
            French fr = new French();
            ChineseSimplified cnS = new ChineseSimplified();
            ChineseTraditional cnT = new ChineseTraditional();
            
            List<int> languageCount = new List<int>(new[] {0, 0, 0, 0, 0, 0});
            int index;

            foreach(string s in words)
            {               
                if(eng.WordExists(s, out index))
                {
                    // english is at 0
                    languageCount[0]++;
                }

                if (jp.WordExists(s, out index))
                {
                    // japanese is at 1
                    languageCount[1]++;
                }

                if (es.WordExists(s, out index))
                {
                    // spanish is at 2
                    languageCount[2]++;
                }

                if (cnS.WordExists(s, out index))
                {
                    // chinese simplified is at 3
                    languageCount[3]++;
                }

                if (cnT.WordExists(s, out index) && ! cnS.WordExists(s, out index))
                {
                    // chinese traditional is at 4
                    languageCount[4]++;
                }

                if (fr.WordExists(s, out index))
                {
                    // french is at 5
                    languageCount[5]++;
                }
            }

            // no hits found for any language unknown
            if(languageCount.Max()==0)
            {
                return Language.Unknown;
            }

            if(languageCount.IndexOf(languageCount.Max()) == 0)
            {
                return Language.English;
            }

            if (languageCount.IndexOf(languageCount.Max()) == 1)
            {
                return Language.Japanese;
            }

            if (languageCount.IndexOf(languageCount.Max()) == 2)
            {
                return Language.Spanish;
            }

            if (languageCount.IndexOf(languageCount.Max()) == 3)
            {
                if (languageCount[4]>0)
                {
                    // has traditional characters so not simplified but instead traditional
                    return Language.ChineseTraditional;
                }

                return Language.ChineseSimplified;
            }

            if (languageCount.IndexOf(languageCount.Max()) == 4)
            {
                return Language.ChineseTraditional;
            }

            if (languageCount.IndexOf(languageCount.Max()) == 5)
            {
                return Language.French;
            }

            return Language.Unknown;
        }

        /// <summary>
        /// Supply a mnemonic sentence with any words of your choosing not restricted to wordlists and be given seed bytes in return
        /// </summary>
        /// <param name="mnemonicSentence">
        /// The mnemonic sentence we will use to derive seed bytes, Please ensure NFKD Normalized
        /// </param>
        /// <param name="passphrase">
        /// Optional passphrase to protect the seed bytes, Please ensure NFKD Normalized, defaults to empty string
        /// </param>
        /// <returns>
        /// Seed bytes that can be used to create a root in BIP32
        /// </returns>
        public static byte[] GetSeedBytes(string mnemonicSentence, string passphrase=CEmptyString)
        {
            mnemonicSentence = mnemonicSentence.Normalize(NormalizationForm.FormKD);
            var salt = Encoding.UTF8.GetBytes(CSaltHeader).Concat(Encoding.UTF8.GetBytes(passphrase.Normalize(NormalizationForm.FormKD))).ToArray();
            return Bip39Kdf.Pbkdf2(Encoding.UTF8.GetBytes(mnemonicSentence), salt);
        }

        /// <summary>
        /// Supply a mnemonic sentence with any words of your choosing not restricted to wordlists and be given seed bytes hex encoded as a string in return
        /// </summary>
        /// <param name="mnemonicSentence">
        /// The mnemonic sentence we will use to derive seed bytes
        /// </param>
        /// <param name="passphrase">
        /// Optional pass phrase to protect the seed bytes, defaults to empty string
        /// </param>
        /// <returns>
        /// Hex string encoded seed bytes that can be used to create a root in BIP32
        /// </returns>
        public static string GetSeedBytesHexString(string mnemonicSentence, string passphrase = CEmptyString)
        {
            return CryptoUtil.ToHex(GetSeedBytes(mnemonicSentence, passphrase)).ToLower();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Common initialisation code utilised by all the constructors. It gets all the bits and does a checksum etc. This is the main code to create a BIP39 object.
        /// </summary>
        /// <param name="passphrase">
        /// The pass phrase.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        private void Init(string passphrase, Language language)
        {
            this.passphraseBytes = Encoding.UTF8.GetBytes(passphrase.Normalize(NormalizationForm.FormKD));

            this.language = language;
            byte[] allChecksumBytes = CryptoUtil.Sha256Hash(this.EntropyBytes); // sha256 the entropy bytes to get all the checksum bits

            this.EntropyBytes = BitHelper.SwapEndianBytes(this.EntropyBytes); //seems I had to change the endianess of the bytes here to match the test vectors.....
            int numberOfChecksumBits = (this.EntropyBytes.Length * CBitsInByte) / CEntropyMultiple; // number of bits to take from the checksum bits, varies on entropy size as per spec
            BitArray entropyConcatChecksumBits = new BitArray((this.EntropyBytes.Length * CBitsInByte) + numberOfChecksumBits);

            allChecksumBytes = BitHelper.SwapEndianBytes(allChecksumBytes); //yet another endianess change of some different bytes to match the test vectors.....             
            int index=0;

            foreach(bool b in new BitArray(this.EntropyBytes))
            {
                entropyConcatChecksumBits.Set(index, b);
                index++;
            }

            /*sooooo I'm doing below for future proofing....I know right now we are using up to 256 bits entropy in real world implementation and therefore max 8 bits (1 byte) of checksum....buuuut I figgure it's easy enough
            to accommodate more entropy by chaining more checksum bytes so maximum 256 * 32 = 8192 theoretical maximum entropy (plus CS).*/
            List<byte> checksumBytesToUse = new List<byte>();

            double byteCount = Math.Ceiling((double)numberOfChecksumBits / CBitsInByte);

            for (int i = 0; i < byteCount ;i++)
            {
                checksumBytesToUse.Add(allChecksumBytes[i]);
            }

            BitArray ba = new BitArray(checksumBytesToUse.ToArray());

            // add checksum bits
            for(int i = 0;i<numberOfChecksumBits;i++)
            {
                entropyConcatChecksumBits.Set(index, ba.Get(i));
                index++;
            }

            this.wordIndexList = this.GetWordIndeces(entropyConcatChecksumBits);
            this.mnemonicSentence = this.GetMnemonicSentence();
   
        }

        /// <summary>
        /// Uses the Wordlist Index to create a sentence ow words provided by the wordlist of this objects language attribute
        /// </summary>
        /// <returns>A sentence of words</returns>
        private string GetMnemonicSentence()
        {
            // trap for words that were not in the word list when built. If custom words were used, we will not support the rebuild as we don't have the words
            if (this.wordIndexList.Contains(-1))
            {
                throw new MnemonicException("the wordlist index contains -1 which means words were used in the mnemonic sentence that cannot be found in the wordlist and the index to sentence feature cannot be used. Perhaps a different language wordlist is needed?");
            }

            string mSentence = CEmptyString;
            Wordlist wordlist;            

            switch(this.language)
            {
                case Language.English:
                    wordlist = new English();
                    break;

                case Language.Japanese:
                    wordlist = new Japanese();
                    break;

                case Language.Spanish:
                    wordlist = new Spanish();
                    break;

                case Language.ChineseSimplified:
                    wordlist = new ChineseSimplified();
                    break;

                case Language.ChineseTraditional:
                    wordlist = new ChineseTraditional();
                    break;

                case Language.French:
                    wordlist = new French();
                    break;

                default:
                    wordlist = new English();
                    break;
            }

            for(int i =0; i<this.wordIndexList.Count;i++)
            {
                mSentence += wordlist.GetWordAtIndex(this.wordIndexList[i]);
                if(i+1 < this.wordIndexList.Count)
                {
                    mSentence += " ";
                }
            }           

            return mSentence;
        }

        /// <summary>
        /// Process entropy + CS into an index list of words to get from wordlist
        /// </summary>
        /// <param name="entropyConcatChecksumBits">
        /// The entropy Concat Checksum Bits.
        /// </param>
        /// <returns>
        /// An index, each int is a line in the wiordlist for the language of choice
        /// </returns>
        private List<int> GetWordIndeces(BitArray entropyConcatChecksumBits)
        {
            List<int> wordIndexList = new List<int>();

            // yea....loop in a loop....what of it!!! Outer loop is segregating bits into 11 bit groups and the inner loop is processing the 11 bits before sending them to be encoded as an int.
            for(int i = 0; i< entropyConcatChecksumBits.Length; i=i+CBitGroupSize)
            {
                BitArray toInt = new BitArray(CBitGroupSize);
                for (int i2 = 0; i2 < CBitGroupSize && i<entropyConcatChecksumBits.Length; i2++)
                {
                    toInt.Set(i2, entropyConcatChecksumBits.Get(i+i2));
                }

                wordIndexList.Add(this.ProcessBitsToInt(toInt)); // adding encoded int to word index               
            }

            return wordIndexList;
        }

        /// <summary>
        /// Takes in the words of a mnemonic sentence and it rebuilds the word index, having the valid index allows us to hot swap between languages/word lists :)
        /// </summary>
        /// <param name="wordsInMnemonicSentence">
        /// a string array containing each word in the mnemonic sentence
        /// </param>
        /// <returns>
        /// The word index that can be used to build the mnemonic sentence
        /// </returns>
        private List<int> RebuildWordIndexes(string[] wordsInMnemonicSentence)
        {
            List<int> wordIndexList = new List<int>();
            string langName = CEmptyString;

            Wordlist wordlist;

            switch (this.language)
            {
                case Language.English:
                    wordlist = new English();
                    langName = "English";
                    break;

                case Language.Japanese:
                    wordlist = new Japanese();
                    langName = "Japanese";
                    break;

                case Language.Spanish:
                    wordlist = new Spanish();
                    langName = "Spanish";
                    break;

                case Language.ChineseSimplified:
                    wordlist = new ChineseSimplified();
                    langName = "Chinese Simplified";
                    break;

                case Language.ChineseTraditional:
                    wordlist = new ChineseTraditional();
                    langName = "Chinese Traditional";
                    break;

                case Language.French:
                    wordlist = new French();
                    langName = "French";
                    break;

                default:
                    wordlist = new English();
                    langName = "English";
                    break;
            }

            foreach(string s in wordsInMnemonicSentence)
            {
                int idx=-1;                

                if(!wordlist.WordExists(s, out idx))
                {
                    throw new MnemonicException("Word " + s + " is not in the wordlist for language " + langName + " cannot continue to rebuild entropy from wordlist");
                }

                wordIndexList.Add(idx);            
            }

            return wordIndexList;
        }

        /// <summary>
        /// Me encoding an integer between 0 and 2047 from 11 bits...
        /// </summary>
        /// <param name="bits">
        /// The bits to encode into an integer
        /// </param>
        /// <returns>
        /// integer between 0 and 2047
        /// </returns>
        private int ProcessBitsToInt(BitArray bits)
        {

            if(bits.Length != CBitGroupSize)
            {
                // to do throw not 11 bits exception
            }

            int number = 0;
            int base2Divide = 1024; // it's all downhill from here...literally we halve this for each bit we move to.

            // literally picture this loop as going from the most significant bit across to the least in the 11 bits, dividing by 2 for each bit as per binary/base 2
            foreach(bool b in bits)
            {
                if(b)
                {
                    number = number + base2Divide;
                }

                base2Divide = base2Divide / 2;
            }            

            return number;
        }

        /// <summary>
        /// Takes the word index and decodes it from our 11 bit integer encoding back into raw bits including CS. Then it removes CS bits and turns back into entropy bytes
        /// </summary>
        /// <param name="wordIndex">
        /// The word index to convert back to bits then bytes
        /// </param>
        /// <returns>
        /// entropy bytes excluding CS
        /// </returns>
        private byte[] ProcessIntToBitsThenBytes(List<int> wordIndex)
        {
            // trap for words that were not in the word list when built. If custom words were used, we will not support the rebuild as we don't have the words
            if (wordIndex.Contains(-1))
            {
                throw new MnemonicException("the wordlist index contains -1 which means words were used in the mnemonic sentence that cannot be found in the wordlist and so the -1 will stuff up our entropy bits and we cannot rebuild the entropy from index containing -1. Perhaps a different language wordlist is needed?");
            }

            BitArray bits = new BitArray(wordIndex.Count * CBitGroupSize);
            
            int bitIndex = 0;            

            // hey look it's another loop in a loop w00t! I'm sure my old uni lecturer is fizzin' at the bumhole with rage somewhere right now.....it works tho :)
            for(int i=0; i< wordIndex.Count;i++)
            {
                double wordindex = wordIndex[i];

                // slide down our 11 bits doin mod 2 to determin true or false for each bit
                for (int biti = 0; biti < 11; biti++)
                {
                    bits[bitIndex] = false;

                    if (wordindex % 2 == 1)
                    {
                        bits[bitIndex] = true;
                    }

                    wordindex = Math.Floor(wordindex / 2);

                    bitIndex++;
                }

                // below swaps the endianess of our 11 bit group.....crude but working
                bool temp = bits.Get(bitIndex - CBitGroupSize);
                bits.Set(bitIndex - CBitGroupSize, bits.Get(bitIndex - 1));
                bits.Set(bitIndex - 1, temp);
                temp = bits.Get(bitIndex - (CBitGroupSize - 1));
                bits.Set(bitIndex - (CBitGroupSize - 1), bits.Get(bitIndex - 2));
                bits.Set(bitIndex - 2, temp);
                temp = bits.Get(bitIndex - (CBitGroupSize - 2));
                bits.Set(bitIndex - (CBitGroupSize - 2), bits.Get(bitIndex - 3));
                bits.Set(bitIndex - 3, temp);
                temp = bits.Get(bitIndex - (CBitGroupSize - 3));
                bits.Set(bitIndex - (CBitGroupSize - 3), bits.Get(bitIndex - 4));
                bits.Set(bitIndex - 4, temp);
                temp = bits.Get(bitIndex - (CBitGroupSize - 4));
                bits.Set(bitIndex - (CBitGroupSize - 4), bits.Get(bitIndex - 5));
                bits.Set(bitIndex - 5, temp);

                //// end bit swappy, rubber fanny haha
            }

            //// now we need to strip the checksum and return entropy bytes
            int length = bits.Length - (bits.Length / (CEntropyMultiple + 1));

            if (length % 8 != 0)
            {
                throw new MnemonicException("Entropy bits less checksum need to be cleanly divisible by " + CBitsInByte);
            }

            byte[] entropy = new byte[length / CBitsInByte];
            BitArray checksum = new BitArray(bits.Length / (CEntropyMultiple + 1));
            BitArray checksumActual = new BitArray(bits.Length / (CEntropyMultiple + 1));

            int index = 0;

            //// get entropy bytes
            for (int byteIndex = 0; byteIndex < entropy.Length; byteIndex++)
            {
                for(int i = 0; i < CBitsInByte; i++)
                {
                    int bitIdx = index % CBitsInByte;
                    byte mask = (byte)(1 << bitIdx);
                    entropy[byteIndex] = (byte)(bits.Get(index) ? (entropy[byteIndex] | mask) : (entropy[byteIndex] & ~mask));
                    index++;
                }
            }

            //// get remaining bits as checksum bits
            int csindex = 0;

            while(index < bits.Length)
            {
                checksum.Set(csindex, bits.Get(index));
                csindex++;
                index++;
            }

            //// now we get actual checksum of our entropy bytes
            BitArray allChecksumBits = new BitArray(BitHelper.SwapEndianBytes(CryptoUtil.Sha256Hash(BitHelper.SwapEndianBytes(entropy)))); //sha256 the entropy bytes to get all the checksum bits

            for(int i=0; i<checksumActual.Length;i++)
            {
                checksumActual.Set(i, allChecksumBits.Get(i));
            }

            //// now we check that our checksum derived from the word index mantches the checksum of our entropy bytes, if so happy days
            foreach(bool b in checksumActual.Xor(checksum))
            {
                if (b)
                {
                    throw new MnemonicException("woah! the checksum I derived from the word index DOES NOT match the checksum I computed from the entropy bytes! We have a problem!");
                }
            }

            return entropy;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the entropy bytes, they can be used to reconstruct this object, providing these bytes and pass phrase is all that is needed
        /// </summary>
        public byte[] EntropyBytes { get; private set; }

        /// <summary>
        /// Sets the pass phrase, this lets us use the same entropy bits to derive many seeds based on different pass phrases
        /// </summary>
        public string Passphrase
        {
            set
            {
                this.passphraseBytes = Encoding.UTF8.GetBytes(value.Normalize(NormalizationForm.FormKD));
            }
        }
        

        /// <summary>
        /// Gets the mnemonic sentence built from ent+cs
        /// </summary>
        public string MnemonicSentence
        {
            get
            {
                string outputMnemonic = this.mnemonicSentence;

                if(this.language.Equals(Language.Japanese))
                {
                    char japSpace;
                    char.TryParse(CJpSpaceString, out japSpace);
                    outputMnemonic = outputMnemonic.Replace(' ', japSpace);
                }
                
                return outputMnemonic;
            }
        }

        /// <summary>
        /// Gets or Sets the language that will be used to provide the mnemonic sentence, WARNING ensure you get new seed bytes after setting language
        /// </summary>
        public Language WordlistLanguage
        {
            get
            {
                return this.language;
            }

            set
            {
                this.language = value;

                ////new language means we need a mnemonic sentence in that language
                this.mnemonicSentence = this.GetMnemonicSentence();
            }
        }

        /// <summary>
        /// Gets the bytes of the seed created from the mnemonic sentence. This could become your root in BIP32
        /// </summary>
        public byte[] SeedBytes
        {
            get
            {
                // literally this is the bulk of the decoupled seed generation code, easy.
                var salt = Encoding.UTF8.GetBytes(CSaltHeader).ToArray().Concat(this.passphraseBytes.ToArray()).ToArray();
                return Bip39Kdf.Pbkdf2(Encoding.UTF8.GetBytes(this.MnemonicSentence.Normalize()), salt);
            }
        }

        /// <summary>
        /// Gets a hex encoded string of the seed bytes
        /// </summary>
        public string SeedBytesHexString
        {
            get
            {
                return CryptoUtil.ToHex(this.SeedBytes).ToLower();
            }
        }

        

        /// <summary>
        /// Gets a count of the words that the entropy will produce
        /// </summary>
        public int WordCountFromEntropy
        {
            get
            {
                int entropyBits = this.EntropyBytes.Length * CBitsInByte;
                return (entropyBits + (entropyBits / CEntropyMultiple)) / CBitGroupSize;

            }
        }

        #endregion
    }
}
