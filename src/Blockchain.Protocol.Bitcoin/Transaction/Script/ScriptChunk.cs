// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptChunk.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Transaction.Script
{
    #region Using Directives

    using System.IO;
    using System.Text;

    using Blockchain.Protocol.Bitcoin.Extension;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;

    #endregion

    /// <summary>
    /// Borrowed from bitcoinj implementation
    /// </summary>
    public class ScriptChunk
    {
        public readonly int Opcode;

        public readonly byte[] Data;

        private readonly long startLocationInProgram;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptChunk"/> class.
        /// </summary>
        public ScriptChunk(int opcode, byte[] data)
            : this(opcode, data, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptChunk"/> class.
        /// </summary>
        public ScriptChunk(int opcode, byte[] data, long startLocationInProgram)
        {
            this.Opcode = opcode;
            this.Data = data;
            this.startLocationInProgram = startLocationInProgram;
        }

        /// <summary>
        /// If this chunk is a single byte of non-pushdata content (could be ScriptOpCodes.OP_RESERVED or some invalid Opcode)
        /// </summary>
        public bool EqualsOpCode(int opcode)
        {
            return opcode == this.Opcode;
        }

        public bool IsOpCode()
        {
            return this.Opcode > ScriptOpCodes.OP_PUSHDATA4;
        }

        /// <summary>
        /// Returns true if this chunk is pushdata content, including the single-byte pushdatas.
        /// </summary>
        public bool IsPushData()
        {
            return this.Opcode <= ScriptOpCodes.OP_16;
        }

        public long GetStartLocationInProgram()
        {
            Guard.Require(this.startLocationInProgram >= 0);
            return this.startLocationInProgram;
        }

        public void Write(BinaryWriter writer)
        {
            if (this.IsOpCode())
            {
                Guard.Require(this.Data == null);
                writer.Write((byte)this.Opcode);
            }
            else if (this.Data != null)
            {
                if (this.Opcode < ScriptOpCodes.OP_PUSHDATA1)
                {
                    Guard.Require(this.Data.Length == this.Opcode);
                    writer.Write((byte)this.Opcode);
                }
                else if (this.Opcode == ScriptOpCodes.OP_PUSHDATA1)
                {
                    Guard.Require(this.Data.Length <= 0xFF);
                    writer.Write((byte)ScriptOpCodes.OP_PUSHDATA1);
                    writer.Write((byte)this.Data.Length);
                }
                else if (this.Opcode == ScriptOpCodes.OP_PUSHDATA2)
                {
                    Guard.Require(this.Data.Length <= 0xFFFF);
                    writer.Write((byte)ScriptOpCodes.OP_PUSHDATA2);
                    ////writer.Write(0xFF & this.Data.Length);
                    ////writer.Write(0xFF & (this.Data.Length >> 8));
                    writer.Write((short)this.Data.Length);
                }
                else if (this.Opcode == ScriptOpCodes.OP_PUSHDATA4)
                {
                    Guard.Require(this.Data.Length <= Script.MAX_SCRIPT_ELEMENT_SIZE);
                    writer.Write((byte)ScriptOpCodes.OP_PUSHDATA4);
                    writer.Write(this.Data.Length);
                }
                else
                {
                    throw new ScriptException("Unimplemented");
                }

                writer.Write(this.Data);
            }
            else
            {
                writer.Write((byte)this.Opcode); // SmallNum
            }
        }

        public override string ToString()
        {

            var builder = new StringBuilder();
            if (this.IsOpCode())
            {
                builder.Append(ScriptOpCodes.GetOpCodeName(this.Opcode));
            }
            else if (this.Data != null)
            {
                // Data chunk
                builder.Append(ScriptOpCodes.GetPushDataName(this.Opcode)).Append("[").Append(CryptoUtil.ToHex(this.Data)).Append("]");
            }
            else
            {
                // Small num
                builder.Append(Script.DecodeFromOpN(this.Opcode));
            }

            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            ScriptChunk other = (ScriptChunk)obj;
            return this.Opcode == other.Opcode && this.startLocationInProgram == other.startLocationInProgram && this.Data.Equals(other.Data);
        }

        public override int GetHashCode()
        {
            return this.Opcode.GetHashCode() + this.startLocationInProgram.GetHashCode() + this.Data.GetHashCode();
        }
    }
}
