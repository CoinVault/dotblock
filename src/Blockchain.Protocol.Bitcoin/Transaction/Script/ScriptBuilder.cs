// <copyright file="ScriptBuilder.cs" company="SoftChains">
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
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Extension;

    #endregion

    /// <summary>
    /// Borrowed from bitcoinj implementation
    /// </summary>
    public class ScriptBuilder
    {
        private readonly List<ScriptChunk> chunks;

        /// <summary>
        /// Creates a fresh ScriptBuilder with an empty program.
        /// </summary>
        public ScriptBuilder()
        {
            this.chunks = new List<ScriptChunk>();
        }

        /// <summary>
        /// Creates a fresh ScriptBuilder with the given program as the starting point. 
        /// </summary>
        public ScriptBuilder(Script template)
        {
            this.chunks = new List<ScriptChunk>(template.Chunks);
        }
         
        /// <summary>
        /// Adds the given chunk to the end of the program
        /// </summary> 
        public ScriptBuilder AddChunk(ScriptChunk chunk)
        {
            return this.AddChunk(this.chunks.Count(), chunk);
        }
         
        /// <summary>
        /// Adds the given chunk at the given index in the program  
        /// </summary>
        public ScriptBuilder AddChunk(int index, ScriptChunk chunk)
        {
            this.chunks.Insert(index, chunk);
            return this;
        }
         
        /// <summary>
        /// Adds the given opcode to the end of the program. 
        /// </summary>
        public ScriptBuilder Op(int opcode)
        {
            return this.Op(this.chunks.Count(), opcode);
        }

        /// <summary>
        /// Adds the given opcode to the given index in the program 
        /// </summary>
        public ScriptBuilder Op(int index, int opcode)
        {
            Guard.Require(opcode > ScriptOpCodes.OP_PUSHDATA4);
            return this.AddChunk(index, new ScriptChunk(opcode, null));
        }

        /// <summary>
        /// Adds a copy of the given byte array as a data element (i.e. PUSHDATA) at the end of the program. 
        /// </summary>
        public ScriptBuilder Data(byte[] data)
        {
            if (data.Length == 0) return this.SmallNum(0);
            return this.Data(this.chunks.Count(), data);
        }

        /// <summary>
        /// Adds a copy of the given byte array as a data element (i.e. PUSHDATA) at the given index in the program.  
        /// </summary>
        public ScriptBuilder Data(int index, byte[] data)
        {
            // implements BIP62
            byte[] copy = data.Clone() as byte[];
            int opcode;
            if (data.Length == 0)
            {
                opcode = ScriptOpCodes.OP_0;
            }
            else if (data.Length == 1)
            {
                byte b = data[0];
                if (b >= 1 && b <= 16) opcode = Script.EncodeToOpN(b);
                else opcode = 1;
            }
            else if (data.Length < ScriptOpCodes.OP_PUSHDATA1)
            {
                opcode = data.Length;
            }
            else if (data.Length < 256)
            {
                opcode = ScriptOpCodes.OP_PUSHDATA1;
            }
            else if (data.Length < 65536)
            {
                opcode = ScriptOpCodes.OP_PUSHDATA2;
            }
            else
            {
                throw new ScriptException("Unimplemented");
            }

            return this.AddChunk(index, new ScriptChunk(opcode, copy));
        }

        /// <summary>
        /// Adds the given number to the end of the program. Automatically uses
        /// shortest encoding possible. 
        /// </summary>
        public ScriptBuilder Number(long num)
        {
            if (num >= 0 && num < 16)
            {
                return this.SmallNum((int)num);
            }

            return this.BigNum(num);
        }

        /// <summary>
        /// Adds the given number to the given index in the program. Automatically
        /// uses shortest encoding possible. 
        /// </summary>
        public ScriptBuilder Number(int index, long num)
        {
            if (num >= 0 && num < 16)
            {
                return this.AddChunk(index, new ScriptChunk(Script.EncodeToOpN((int)num), null));
            }

            return this.BigNum(index, num);
        }

        /// <summary>
        /// Adds the given number as a OP_N opcode to the end of the program.
        /// Only handles values 0-16 inclusive. 
        /// </summary>     
        public ScriptBuilder SmallNum(int num)
        {
            return this.SmallNum(this.chunks.Count(), num);
        }

        /// <summary>
        /// Adds the given number as a push data chunk.
        /// This is intended to use for negative numbers or values > 16, and although
        /// it will accept numbers in the range 0-16 inclusive, the encoding would be
        /// considered non-standard. 
        /// </summary>
        protected ScriptBuilder BigNum(long num)
        {
            return this.BigNum(this.chunks.Count(), num);
        }

        /// <summary>
        /// Adds the given number as a OP_N opcode to the given index in the program.
        /// Only handles values 0-16 inclusive. 
        /// </summary>
        public ScriptBuilder SmallNum(int index, int num)
        {
            Guard.Require(num >= 0, "Cannot encode negative numbers with SmallNum");
            Guard.Require(num <= 16, "Cannot encode numbers larger than 16 with SmallNum");
            return this.AddChunk(index, new ScriptChunk(Script.EncodeToOpN(num), null));
        }

        /// <summary>
        /// Adds the given number as a push data chunk to the given index in the program.
        /// This is intended to use for negative numbers or values > 16, and although
        /// it will accept numbers in the range 0-16 inclusive, the encoding would be
        /// considered non-standard. 
        /// </summary>
        protected ScriptBuilder BigNum(int index, long num)
        {
            byte[] data;

            if (num == 0)
            {
                data = new byte[0];
            }
            else
            {
                Stack<byte> result = new Stack<byte>();
                bool neg = num < 0;
                long absvalue = Math.Abs(num);

                while (absvalue != 0)
                {
                    result.Push((byte)(absvalue & 0xff));
                    absvalue >>= 8;
                }

                if ((result.Peek() & 0x80) != 0)
                {
                    // The most significant byte is >= 0x80, so push an extra byte that
                    // contains just the sign of the value.
                    result.Push((byte)(neg ? 0x80 : 0));
                }
                else if (neg)
                {
                    // The most significant byte is < 0x80 and the value is negative,
                    // set the sign bit so it is subtracted and interpreted as a
                    // negative when converting back to an integral.
                    result.Push((byte)(result.Pop() | 0x80));
                }

                data = new byte[result.Count()];
                for (int byteIdx = 0; byteIdx < data.Length; byteIdx++)
                {
                    data[byteIdx] = result.ElementAt(byteIdx);
                }

                // todo: investigate why this is required
                data = data.Reverse().ToArray();

                ////if (num <= short.MaxValue)
                ////{
                ////    data = BitConverter.GetBytes((short)num);
                ////}
                ////else if(num <= int.MaxValue)
                ////{
                ////    data = BitConverter.GetBytes((int)num);

                ////}
                ////else
                ////{
                ////    data = BitConverter.GetBytes(num);
                ////}
            }

            // At most the encoded value could take up to 8 bytes, so we don't need
            // to use OP_PUSHDATA opcodes
            return this.AddChunk(index, new ScriptChunk(data.Length, data));
        }

         
        /// <summary>
        /// Creates a new immutable Script based on the state of the builder. 
        /// </summary>
        public Script Build()
        {
            return new Script(this.chunks);
        }

         
        /// <summary>
        /// Creates a scriptPubKey that encodes payment to the given address. 
        /// </summary>
        public static Script CreateOutputScript(Address to)
        {
             if (to.IsScriptAddress()) 
             {
                 // OP_HASH160 <scriptHash> OP_EQUAL
                 return new ScriptBuilder()
                 .Op(ScriptOpCodes.OP_HASH160)
                 .Data(to.Hash160)
                 .Op(ScriptOpCodes.OP_EQUAL)
                 .Build();
             } 
             else
             {
                // OP_DUP OP_HASH160 <pubKeyHash> OP_EQUALVERIFY OP_CHECKSIG
                return new ScriptBuilder()
                    .Op(ScriptOpCodes.OP_DUP)
                    .Op(ScriptOpCodes.OP_HASH160)
                    .Data(to.Hash160)
                    .Op(ScriptOpCodes.OP_EQUALVERIFY)
                    .Op(ScriptOpCodes.OP_CHECKSIG).Build();
            }
        }

        /// <summary>
        /// Creates a scriptPubKey that encodes payment to the given raw public key.  
        /// </summary>
        public static Script CreateOutputScript(EcKey key)
        {
            return new ScriptBuilder().Data(key.PublicKeyBytes).Op(ScriptOpCodes.OP_CHECKSIG).Build();
        }

        /// <summary>
        /// Creates a scriptSig that can redeem a pay-to-address output.
        /// If given signature is null, incomplete scriptSig will be created with ScriptOpCodes.OP_0 instead of signature 
        /// </summary>
        public static Script CreateInputScript(TransactionSignature signature, EcKey pubKey)
        {
            byte[] pubkeyBytes = pubKey.PublicKeyBytes;
            byte[] sigBytes = signature != null ? signature.EncodeToBitcoin() : new byte[] { };
            return new ScriptBuilder().Data(sigBytes).Data(pubkeyBytes).Build();
        }

        /// <summary>
        /// Creates a scriptSig that can redeem a pay-to-pubkey output.
        /// If given signature is null, incomplete scriptSig will be created with OP_0 instead of signature 
        /// </summary>
        public static Script CreateInputScript(TransactionSignature signature)
        {
            byte[] sigBytes = signature != null ? signature.EncodeToBitcoin() : new byte[] { };
            return new ScriptBuilder().Data(sigBytes).Build();
        }
    }
}
