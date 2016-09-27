// <copyright file="TransactionSerializer.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Transaction.Serializers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    #endregion

    /// <summary>
    /// The base class for serialization of a transaction
    /// Functionality of serializing a transaction to and from HEX format.
    /// This class is designed to be overridden by other Blockchain coins that may have different serialization formats.
    /// </summary>
    public class TransactionSerializer
    {
        public CoinParameters Parameters { get; set; }

        public TransactionSerializer(CoinParameters parameters)
        {
            this.Parameters = parameters;
        }

        public virtual string ToHex(Transaction transaction, Transaction.SigHash sigHash = Transaction.SigHash.Undefined)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    this.WriteVersion(writer, transaction);
                    this.WriteTimeStamp(writer, transaction);
                    this.WriteInputs(writer, transaction);
                    this.WriteOutputs(writer, transaction);
                    this.WriteLocktime(writer, transaction);
                   
                    // when signing a transaction
                    this.WriteSigHash(writer, transaction, sigHash);

                    return CryptoUtil.ToHex(stream.ToArray());
                }
            }
        }

        public virtual Transaction FromHex(string rawHexTransaction)
        {
            var data = CryptoUtil.ConvertHex(rawHexTransaction);
            var transaction = new Transaction { Inputs = new List<TransactionInput>(), Outputs = new List<TransactionOutput>() };

            var trxBytes = CryptoUtil.ConvertHex(rawHexTransaction);
            var trxHash = CryptoUtil.Sha256Hash(CryptoUtil.Sha256Hash(trxBytes));
            transaction.Hash = CryptoUtil.ToHex(trxHash.Reverse().ToArray());

            using (var stream = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(stream))
                {
                    this.ReadVersion(reader, transaction);
                    this.ReadTimeStamp(reader, transaction);
                    this.ReadInputs(reader, transaction);
                    this.ReadOutputs(reader, transaction);
                    this.ReadLocktime(reader,transaction);
                }
            }

            return transaction;
        }

        protected static void WriteCompactSize(BinaryWriter writer, long size)
        {
            if (size < 253)
            {
                writer.Write(Convert.ToByte(size));
            }
            else if (size <= ushort.MaxValue)
            {
                writer.Write(Convert.ToByte(253));
                writer.Write(Convert.ToUInt16(size));
            }
            else if (size <= uint.MaxValue)
            {
                writer.Write(Convert.ToByte(254));
                writer.Write(Convert.ToUInt32(size));
            }
            else
            {
                writer.Write(Convert.ToByte(255));
                writer.Write(Convert.ToUInt64(size));
            }
        }

        protected static ulong ReadCompactSize(BinaryReader reader)
        {
            var size = reader.ReadByte();
            
            ulong sizeRet = 0;
            if (size < 253)
            {
                sizeRet = size;
            }
            else if (size == 253)
            {
                sizeRet = reader.ReadUInt16();
                if (sizeRet < 253)
                {
                    throw new TransactionException("non-canonical");
                }
            }
            else if (size == 254)
            {
                sizeRet = reader.ReadUInt32();
                if (sizeRet < 0x10000)
                {
                    throw new TransactionException("non-canonical");
                }
            }
            else
            {
                sizeRet = reader.ReadUInt64();

                if (sizeRet < 0x100000000)
                {
                    throw new TransactionException("non-canonical");
                }
            }

            return sizeRet;
        }

        public virtual decimal NumToValue(long num)
        {
            return Convert.ToDecimal(num) / this.Parameters.CoinScale;
        }

        public virtual long ValueToNum(decimal num)
        {
            return Convert.ToInt64(num * this.Parameters.CoinScale);
        }

        protected virtual void ReadOutputs(BinaryReader reader, Transaction transaction)
        {
            var ouputs = ReadCompactSize(reader);
            foreach (var index in Range.UInt64(0, ouputs - 1))
            {
                var output = new TransactionOutput();

                output.Index = (int)index;
                output.Value = reader.ReadInt64();
                var scriptLen = ReadCompactSize(reader);
                output.ScriptBytes = reader.ReadBytes((int)scriptLen);

                transaction.Outputs.Add(output);
            }
        }

        protected virtual void WriteOutputs(BinaryWriter writer, Transaction transaction)
        {
            WriteCompactSize(writer, transaction.Outputs.Count());
            foreach (var vout in transaction.Outputs)
            {
                writer.Write(vout.Value);
                WriteCompactSize(writer, vout.ScriptBytes.Length);
                writer.Write(vout.ScriptBytes);
            }
        }

        protected virtual void ReadInputs(BinaryReader reader, Transaction transaction)
        {
            var inputs = ReadCompactSize(reader);
            foreach (var index in Range.UInt64(0, inputs - 1))
            {
                var input = new TransactionInput { Outpoint = new TransactionOutPoint() };

                var hash = reader.ReadBytes(32);
                input.Outpoint.Hash = CryptoUtil.ToHex(hash.Reverse().ToArray());
                input.Outpoint.Index = BitHelper.ToUInt32(reader.ReadBytes(4));
                var scriptLen = ReadCompactSize(reader);
                input.ScriptBytes = reader.ReadBytes((int)scriptLen);
                input.Sequence = BitHelper.ToUInt32(reader.ReadBytes(4));

                transaction.Inputs.Add(input);
            }
        }

        protected virtual void WriteInputs(BinaryWriter writer, Transaction transaction)
        {
            WriteCompactSize(writer, transaction.Inputs.Count());
            foreach (var input in transaction.Inputs)
            {
                writer.Write(CryptoUtil.ConvertHex(input.Outpoint.Hash).Reverse().ToArray());
                writer.Write(Convert.ToUInt32(input.Outpoint.Index));
                WriteCompactSize(writer, input.ScriptBytes.Length);
                writer.Write(input.ScriptBytes);
                writer.Write(Convert.ToUInt32(input.Sequence));
            }
        }

        protected virtual void ReadVersion(BinaryReader reader, Transaction transaction)
        {
            transaction.Version = (int)BitHelper.ToUInt32(reader.ReadBytes(4));
        }

        protected virtual void WriteVersion(BinaryWriter writer, Transaction transaction)
        {
            writer.Write(Convert.ToUInt32(transaction.Version));
        }

        protected virtual void ReadLocktime(BinaryReader reader, Transaction transaction)
        {
            transaction.Locktime = BitHelper.ToUInt32(reader.ReadBytes(4));
        }

        protected virtual void WriteLocktime(BinaryWriter writer, Transaction transaction)
        {
            writer.Write(Convert.ToUInt32(transaction.Locktime));
        }

        protected virtual void WriteSigHash(BinaryWriter writer, Transaction transactionm, Transaction.SigHash sigHash)
        {
            if (sigHash != Transaction.SigHash.Undefined)
            {
                writer.Write((uint)sigHash);
            }
        }

        protected virtual void ReadTimeStamp(BinaryReader reader, Transaction trx)
        {
        }

        protected virtual void WriteTimeStamp(BinaryWriter writer, Transaction trx)
        {
        }
    }
}
