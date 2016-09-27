// <copyright file="Script.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Transaction.Script
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;

    using Org.BouncyCastle.Utilities;

    #endregion

    /// <summary>
    /// Borrowed from bitcoinj implementation
    /// </summary>
    public class Script
    {
        /// <summary>
        /// The script type.
        /// </summary>
        public enum ScriptType
        {
            // Do NOT change the ordering of the following definitions because their ordinals are stored in databases.
            NO_TYPE, 

            P2PKH, 

            PUB_KEY, 

            P2SH
        }

        public enum VerifyFlag
        {
            /// <summary>
            /// The p 2 sh.
            /// </summary>
            P2SH, // Enable BIP16-style subscript evaluation.

            /// <summary>
            /// The strictenc.
            /// </summary>
            STRICTENC, // Passing a non-strict-DER signature or one with undefined hashtype to a checksig operation causes script failure.

            /// <summary>
            /// The dersig.
            /// </summary>
            DERSIG, // Passing a non-strict-DER signature to a checksig operation causes script failure (softfork safe, BIP66 rule 1)

            /// <summary>
            /// The lo w_ s.
            /// </summary>
            LOW_S, // Passing a non-strict-DER signature or one with S > order/2 to a checksig operation causes script failure

            /// <summary>
            /// The nulldummy.
            /// </summary>
            NULLDUMMY, // Verify dummy stack item consumed by CHECKMULTISIG is of zero-length.

            /// <summary>
            /// The sigpushonly.
            /// </summary>
            SIGPUSHONLY, // Using a non-push operator in the scriptSig causes script failure (softfork safe, BIP62 rule 2).

            /// <summary>
            /// The minimaldata.
            /// </summary>
            MINIMALDATA, // Require minimal encodings for all push operations

            /// <summary>
            /// The discourag e_ upgradabl e_ nops.
            /// </summary>
            DISCOURAGE_UPGRADABLE_NOPS, // Discourage use of NOPs reserved for upgrades (NOP1-10)

            /// <summary>
            /// The cleanstack.
            /// </summary>
            CLEANSTACK, // Require that only a single stack element remains after evaluation.

            /// <summary>
            /// The checklocktimeverify.
            /// </summary>
            CHECKLOCKTIMEVERIFY // Enable CHECKLOCKTIMEVERIFY operation
        }

        public static long MAX_SCRIPT_ELEMENT_SIZE = 520; // bytes

        public static int SIG_SIZE = 75;

        public static int MAX_P2SH_SIGOPS = 15;

        /// <summary>
        /// The program is a set of chunks where each element is either [opcode] or [data, data, data ...].
        /// </summary>
        public List<ScriptChunk> Chunks;

        /// <summary>
        /// Unfortunately, scripts are not ever re-serialized or canonicalized when used in signature hashing. Thus we
        /// must preserve the exact bytes that we read off the wire, along with the parsed form.
        /// </summary>
        public byte[] Program;
        
        /// <summary>
        /// Creation time of the associated keys in seconds since the epoch.
        /// </summary>
        private long creationTimeSeconds;

        /// <summary>
        /// Prevents a default instance of the <see cref="Script"/> class from being created.
        /// </summary>
        private Script()
        {
            this.Chunks = new List<ScriptChunk>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        public Script(List<ScriptChunk> chunks)
        {
            this.Chunks = chunks;
            this.creationTimeSeconds = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        public Script(byte[] programBytes)
        {
            this.Program = programBytes;
            this.Parse(programBytes);
            this.creationTimeSeconds = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        public Script(byte[] programBytes, long creationTimeSeconds)
        {
            this.Program = programBytes;
            this.Parse(programBytes);
            this.creationTimeSeconds = creationTimeSeconds;
        }

        /// <summary>
        /// Returns the program opcodes as a string, for example "[1234] DUP HASH160.
        /// </summary>
        public override string ToString()
        {
            return string.Join(" ", this.Chunks);
        }

        /// <summary>
        /// Returns the serialized program as a newly created byte array.
        /// </summary>
        public byte[] GetProgram()
        {
             // Don't round-trip as Bitcoin Core doesn't and it would introduce a mismatch.
            if (this.Program != null)
            {
                return Arrays.Clone(this.Program);
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    foreach (ScriptChunk chunk in this.Chunks)
                    {
                        chunk.Write(writer);
                    }

                    this.Program = stream.ToArray();
                }
            }

            return this.Program;
        }

        /// <summary>
        /// <p>To run a script, first we parse it which breaks it up into chunks representing pushes of data or logical
        /// opcodes. Then we can run the parsed chunks.</p>
        ///
        /// <p>The reason for this split, instead of just interpreting directly, is to make it easier
        /// to reach into a programs structure and pull out bits of data without having to run it.
        /// This is necessary to render the to/from addresses of transactions in a user interface.
        /// Bitcoin Core does something similar.</p>
        /// </summary>
        /// <param name="program">
        /// The program.
        /// </param>
        /// <exception cref="ScriptException">
        /// </exception>
        private void Parse(byte[] program)
        {
            this.Chunks = new List<ScriptChunk>(5); // Common size.
            using (var stream = new MemoryStream(program))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var initialSize = reader.BaseStream.Length;
                    while (reader.Available() > 0)
                    {
                        var startLocationInProgram = reader.BaseStream.Position;
                        int opcode = reader.ReadByte();

                        long dataToRead = -1;
                        if (opcode >= 0 && opcode < ScriptOpCodes.OP_PUSHDATA1)
                        {
                            // Read some bytes of data, where how many is the opcode value itself.
                            dataToRead = opcode;
                        }
                        else if (opcode == ScriptOpCodes.OP_PUSHDATA1)
                        {
                            if (reader.Available() < 1)
                            {
                                throw new ScriptException("Unexpected end of script");
                            }

                            dataToRead = reader.ReadInt16();
                        }
                        else if (opcode == ScriptOpCodes.OP_PUSHDATA2)
                        {
                            // Read a short, then read that many bytes of data.
                            if (reader.Available() < 2)
                            {
                                throw new ScriptException("Unexpected end of script");
                            }

                            dataToRead = reader.ReadInt32();
                        }
                        else if (opcode == ScriptOpCodes.OP_PUSHDATA4)
                        {
                            // Read a uint32, then read that many bytes of data.
                            // Though this is allowed, because its value cannot be > 520, it should never actually be used
                            if (reader.Available() < 4)
                            {
                                throw new ScriptException("Unexpected end of script");
                            }

                            dataToRead = reader.ReadInt64();
                        }

                        ScriptChunk chunk;
                        if (dataToRead == -1)
                        {
                            chunk = new ScriptChunk(opcode, null, startLocationInProgram);
                        }
                        else
                        {
                            if (dataToRead > reader.Available())
                            {
                                throw new ScriptException("Push of data element that is larger than remaining data");
                            }

                            byte[] data = reader.ReadBytes((int)dataToRead);
                            chunk = new ScriptChunk(opcode, data, startLocationInProgram);
                        }

                        this.Chunks.Add(chunk);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if this script is of the form pubkey ScriptOpCodes.ScriptOpCodes.OP_CHECKSIG. This form was originally intended for transactions
        /// where the peers talked to each other directly via TCP/IP, but has fallen out of favour with time due to that mode
        /// of operation being susceptible to man-in-the-middle attacks. It is still used in coinbase outputs and can be
        /// useful more exotic types of transaction, but today most payments are to addresses.
        /// </summary>
        public bool IsSentToRawPubKey()
        {
            return this.Chunks.Count() == 2 
                && this.Chunks.ElementAt(1).EqualsOpCode(ScriptOpCodes.OP_CHECKSIG) 
                && !this.Chunks.ElementAt(0).IsOpCode() 
                && this.Chunks.ElementAt(0).Data.Length > 1;
        }

        /// <summary>
        /// Returns true if this script is of the form DUP HASH160 (pubkey hash) EQUALVERIFY CHECKSIG, ie, payment to an
        /// address like 1VayNert3x1KzbpzMGt2qdqrAThiRovi8. This form was originally intended for the case where you wish
        /// to send somebody money with a written code because their node is offline, but over time has become the standard
        /// way to make payments due to the short and recognizable base58 form addresses come in.
        /// </summary>
        public bool IsSentToAddress()
        {
            return this.Chunks.Count() == 5 
                && this.Chunks.ElementAt(0).EqualsOpCode(ScriptOpCodes.OP_DUP) 
                && this.Chunks.ElementAt(1).EqualsOpCode(ScriptOpCodes.OP_HASH160) 
                && this.Chunks.ElementAt(2).Data.Length == BitcoinPublicKey.Length 
                && this.Chunks.ElementAt(3).EqualsOpCode(ScriptOpCodes.OP_EQUALVERIFY) 
                && this.Chunks.ElementAt(4).EqualsOpCode(ScriptOpCodes.OP_CHECKSIG);
        }

        /// <summary>
        /// <p>If a program matches the standard template DUP HASH160 &lt;pubkey hash&gt; EQUALVERIFY CHECKSIG
        /// then this function retrieves the third element.
        /// In this case, this is useful for fetching the destination address of a transaction.</p>
        /// 
        /// <p>If a program matches the standard template HASH160 &lt;script hash&gt; EQUAL
        /// then this function retrieves the second element.
        /// In this case, this is useful for fetching the hash of the redeem script of a transaction.</p>
        /// <p>Otherwise it throws a ScriptException.</p>
        /// </summary>
        public byte[] GetPubKeyHash()
        {
            if (this.IsSentToAddress())
            {
                return this.Chunks.ElementAt(2).Data;
            }

            if (this.IsPayToScriptHash())
            {
                return this.Chunks.ElementAt(1).Data;
            }

            throw new ScriptException("Script not in the standard scriptPubKey form");
        }

        public byte[] TryGetPubKeyHash()
        {
            if (this.IsSentToAddress())
            {
                return this.Chunks.ElementAt(2).Data;
            }

            if (this.IsPayToScriptHash())
            {
                return this.Chunks.ElementAt(1).Data;
            }

            return null;
        }

        /// <summary>
        /// Returns the public key in this script. If a script contains two constants and nothing else, it is assumed to
        /// be a scriptSig (input) for a pay-to-address output and the second constant is returned (the first is the
        /// signature). If a script contains a constant and an ScriptOpCodes.OP_CHECKSIG opcode, the constant is returned as it is
        /// assumed to be a direct pay-to-key scriptPubKey (output) and the first constant is the public key.
        /// </summary>
        public byte[] GetPubKey()
        {
            Thrower.If(this.Chunks.Count() != 2).Throw<ScriptException>("Script not of right size, expecting 2 but got " + this.Chunks.Count());

            ScriptChunk firstChunk = this.Chunks.ElementAt(0);
            ScriptChunk secondChunk = this.Chunks.ElementAt(1);
            if (firstChunk.Data != null && firstChunk.Data.Length > 2 && secondChunk.Data != null && secondChunk.Data.Length > 2)
            {
                // If we have two large constants assume the input to a pay-to-address output.
                return secondChunk.Data;
            }

            if (secondChunk.EqualsOpCode(ScriptOpCodes.OP_CHECKSIG) && firstChunk.Data != null && firstChunk.Data.Length > 2)
            {
                // A large constant followed by an ScriptOpCodes.OP_CHECKSIG is the key.
                return firstChunk.Data;
            }

            throw new ScriptException("Script did not match expected form: " + this);
        }

        /// <summary>
        /// For 2-element [input] scripts assumes that the paid-to-address can be derived from the public key.
        /// The concept of a "from address" isn't well defined in Bitcoin and you should not assume the sender of a
        /// transaction can actually receive coins on it. This method may be removed in future.
        /// </summary>
        public Address GetFromAddress(CoinParameters param)
        {
            return new BitcoinPublicKey(this.GetPubKey()).ToAddress(param);
        }

        /// <summary>
        /// Gets the destination address from this script, if it's in the required form (see getPubKey).
        /// </summary>
        public Address GetToAddress(CoinParameters param)
        {
            return this.GetToAddress(param, false);
        }

        public static byte[] RemoveAllInstancesOfOp(byte[] inputScript, int opCode)
        {
            return RemoveAllInstancesOf(inputScript, new byte[] { (byte)opCode });
        }

        public static byte[] RemoveAllInstancesOf(byte[] inputScript, byte[] chunkToRemove)
        {
            // We usually don't end up removing anything
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    int cursor = 0;
                    while (cursor < inputScript.Length)
                    {
                        bool skip = EqualsRange(inputScript, cursor, chunkToRemove);

                        int opcode = inputScript[cursor++] & 0xFF;
                        int additionalBytes = 0;
                        if (opcode >= 0 && opcode < ScriptOpCodes.OP_PUSHDATA1)
                        {
                            additionalBytes = opcode;
                        }
                        else if (opcode == ScriptOpCodes.OP_PUSHDATA1)
                        {
                            additionalBytes = (0xFF & inputScript[cursor]) + 1;
                        }
                        else if (opcode == ScriptOpCodes.OP_PUSHDATA2)
                        {
                            additionalBytes = ((0xFF & inputScript[cursor]) | ((0xFF & inputScript[cursor + 1]) << 8)) + 2;
                        }
                        else if (opcode == ScriptOpCodes.OP_PUSHDATA4)
                        {
                            additionalBytes = ((0xFF & inputScript[cursor]) | ((0xFF & inputScript[cursor + 1]) << 8) | ((0xFF & inputScript[cursor + 1]) << 16) | ((0xFF & inputScript[cursor + 1]) << 24)) + 4;
                        }
                        if (!skip)
                        {
                            writer.Write((byte)opcode);
                            writer.Write(inputScript.ToList().GetRange(cursor, additionalBytes).ToArray());

                        }
                        cursor += additionalBytes;
                    }
                    return stream.ToArray();
                }
            }
        }

        private static bool EqualsRange(byte[] a, int start, byte[] b)
        {
            if (start + b.Length > a.Length)
            {
                return false;
            }
            
            for (int i = 0; i < b.Length; i++)
            {
                if (a[i + start] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        public Address GetToAddress(CoinParameters param, bool forcePayToPubKey)
        {
            if (this.IsSentToAddress())
            {
                return Address.Create(param, this.GetPubKeyHash());
            }
            else if (this.IsPayToScriptHash())
            {
                return Address.Create(param, this.GetPubKeyHash());
            }
            else if (forcePayToPubKey && this.IsSentToRawPubKey())
            {
                return new BitcoinPublicKey(this.GetPubKey()).ToAddress(param);
            }
            else
            {
                throw new ScriptException("Cannot cast this script to a pay-to-address type");
            }
        }

        /// <summary>
        /// Writes out the given byte buffer to the output stream with the correct opcode prefix
        /// To write an integer call writeBytes(out, Utils.reverseBytes(Utils.encodeMPI(val, false)));
        /// </summary>
        public static void WriteBytes(BinaryWriter writer, byte[] buf)
        {
            if (buf.Length < ScriptOpCodes.OP_PUSHDATA1)
            {
                writer.Write(buf.Length);
                writer.Write(buf);
            }
            else if (buf.Length < 256)
            {
                writer.Write(ScriptOpCodes.OP_PUSHDATA1);
                writer.Write(buf.Length);
                writer.Write(buf);
            }
            else if (buf.Length < 65536)
            {
                writer.Write(ScriptOpCodes.OP_PUSHDATA2);
                writer.Write(0xFF & buf.Length);
                writer.Write(0xFF & (buf.Length >> 8));
                writer.Write(buf);
            }
            else
            {
                throw new ScriptException("Unimplemented");
            }
        }

        /// <summary>
        /// Creates an incomplete scriptSig that, once filled with signatures, can redeem output containing this scriptPubKey.
        /// Instead of the signatures resulting script has ScriptOpCodes.OP_0.
        /// Having incomplete input script allows to pass around partially signed tx.
        /// It is expected that this program later on will be updated with proper signatures.
        /// </summary>
        public Script CreateEmptyInputScript(EcKey key, Script redeemScript)
        {
            if (this.IsSentToAddress())
            {
                Guard.Require(key != null, "Key required to create pay-to-address input script");
                return ScriptBuilder.CreateInputScript(null, key);
            }

            if (this.IsSentToRawPubKey())
            {
                return ScriptBuilder.CreateInputScript(null);
            }

            if (this.IsPayToScriptHash())
            {
                throw new NotImplementedException();

                ////Guard.Require(redeemScript != null, "Redeem script required to create P2SH input script");
                ////return ScriptBuilder.CreateP2SHMultiSigInputScript(null, redeemScript);
            }

            throw new ScriptException("Do not understand script type: " + this);
        }

        private static int GetSigOpCount(List<ScriptChunk> chunks, bool accurate)
        {
            int sigOps = 0;
            int lastOpCode = ScriptOpCodes.OP_INVALIDOPCODE;
            foreach (ScriptChunk chunk in chunks)
            {
                if (chunk.IsOpCode())
                {
                    switch (chunk.Opcode)
                    {
                        case ScriptOpCodes.OP_CHECKSIG:
                        case ScriptOpCodes.OP_CHECKSIGVERIFY:
                            sigOps++;
                            break;
                        case ScriptOpCodes.OP_CHECKMULTISIG:
                        case ScriptOpCodes.OP_CHECKMULTISIGVERIFY:
                            if (accurate && lastOpCode >= ScriptOpCodes.OP_1 && lastOpCode <= ScriptOpCodes.OP_16)
                            {
                                sigOps += DecodeFromOpN(lastOpCode);
                            }
                            else
                            {
                                sigOps += 20;
                            }

                            break;
                        default:
                            break;
                    }

                    lastOpCode = chunk.Opcode;
                }
            }

            return sigOps;
        }

        public static int DecodeFromOpN(int opcode)
        {
            Guard.Require((opcode == ScriptOpCodes.OP_0 || opcode == ScriptOpCodes.OP_1NEGATE) || (opcode >= ScriptOpCodes.OP_1 && opcode <= ScriptOpCodes.OP_16), "decodeFromOpN called on non ScriptOpCodes.OP_N opcode");
            if (opcode == ScriptOpCodes.OP_0)
            {
                return 0;
            }

            if (opcode == ScriptOpCodes.OP_1NEGATE)
            {
                return -1;
            }

            return opcode + 1 - ScriptOpCodes.OP_1;
        }

        public static int EncodeToOpN(int value)
        {
            Guard.Require(value >= -1 && value <= 16, "encodeToOpN called for " + value + " which we cannot encode in an opcode.");
            if (value == 0)
            {
                return ScriptOpCodes.OP_0;
            }

            if (value == -1)
            {
                return ScriptOpCodes.OP_1NEGATE;
            }

            return value - 1 + ScriptOpCodes.OP_1;
        }

        /// <summary>
        /// Gets the count of regular SigOps in the script program (counting multisig ops as 20)
        /// </summary>
        public static int GetSigOpCount(byte[] program)
        {
            Script script = new Script();
            try
            {
                script.Parse(program);
            }
            catch (ScriptException)
            {
                // Ignore errors and count up to the parse-able length
            }

            return GetSigOpCount(script.Chunks, false);
        }

        public static long GetP2ShSigOpCount(byte[] scriptSig)
        {
            Script script = new Script();
            try
            {
                script.Parse(scriptSig);
            }
            catch (ScriptException)
            {
                // Ignore errors and count up to the parse-able length
            }

            for (int i = script.Chunks.Count() - 1; i >= 0; i--)
            {
                if (!script.Chunks.ElementAt(i).IsOpCode())
                {
                    Script subScript = new Script();
                    subScript.Parse(script.Chunks.ElementAt(i).Data);
                    return GetSigOpCount(subScript.Chunks, true);
                }
            }

            return 0;
        }

        /// <summary>
        /// <p>Whether or not this is a scriptPubKey representing a pay-to-script-hash output. In such outputs, the logic that
        /// controls reclamation is not actually in the output at all. Instead there's just a hash, and it's up to the
        /// spending input to provide a program matching that hash. This rule is "soft enforced" by the network as it does
        /// not exist in Bitcoin Core. It means blocks containing P2SH transactions that don't match
        /// correctly are considered valid, but won't be mined upon, so they'll be rapidly re-orgd out of the chain. This
        /// logic is defined by <a href="https://github.com/bitcoin/bips/blob/master/bip-0016.mediawiki">BIP 16</a>.</p>
        /// </summary>
        public bool IsPayToScriptHash()
        {
            // We have to check against the serialized form because BIP16 defines a P2SH output using an exact byte
            // template, not the logical program structure. Thus you can have two programs that look identical when
            // printed out but one is a P2SH script and the other isn't! :(
            byte[] program = this.GetProgram();
            return program.Length == 23
                && (program[0] & 0xff) == ScriptOpCodes.OP_HASH160
                && (program[1] & 0xff) == 0x14
                && (program[22] & 0xff) == ScriptOpCodes.OP_EQUAL;
        }

        /// <summary>
        /// The is op return.
        /// </summary>
        public bool IsOpReturn()
        {
            return this.Chunks.Any() && this.Chunks.ElementAt(0).EqualsOpCode(ScriptOpCodes.OP_RETURN);
        }

        public ScriptType GetScriptType()
        {
            ScriptType type = ScriptType.NO_TYPE;
            if (this.IsSentToAddress())
            {
                type = ScriptType.P2PKH;
            }
            else if (this.IsSentToRawPubKey())
            {
                type = ScriptType.PUB_KEY;
            }
            else if (this.IsPayToScriptHash())
            {
                type = ScriptType.P2SH;
            }

            return type;
        }
    }
}
