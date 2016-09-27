// <copyright file="Wordlist.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Mnemonic.Wordlists
{
    #region Using Directives

    using System;
    using System.Linq;

    #endregion

    /// <summary>
    /// The wordlist.
    /// </summary>
    public abstract class Wordlist
    {
        /// <summary>
        /// The _words.
        /// </summary>
        private readonly string[] _words;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wordlist"/> class. 
        /// Constructor used by inheritance only
        /// </summary>
        /// <param name="words">
        /// The words to be used in the wordlist
        /// </param>
        public Wordlist(string[] words)
        {
            this._words = words;
        }

        /// <summary>
        /// Method to determine if word exists in word list, great for auto language detection
        /// </summary>
        /// <param name="word">
        /// The word to check for existence
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// Exists (true/false)
        /// </returns>
        public bool WordExists(string word, out int index)
        {
            if(this._words.Contains(word))
            {
                index = Array.IndexOf(this._words, word);
                return true;
            }

            // index -1 means word is not in wordlist
            index = -1;
            return false;
        }

        /// <summary>
        /// Returns a string containing the word at the specified index of the wordlist
        /// </summary>
        /// <param name="index">
        /// Index of word to return
        /// </param>
        /// <returns>
        /// Word
        /// </returns>
        public string GetWordAtIndex(int index)
        {
            return this._words[index];
        }

        /// <summary>
        /// The number of all the words in the wordlist
        /// </summary>
        public int WordCount
        {
            get
            {
                return this._words.Length;
            }
        }        
    }
}
