// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionSerializerReddcoin.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Transaction.Serializers
{
    #region Using Directives

    using System;
    using System.IO;

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    #endregion

    /// <summary>
    /// Serialize a Reddcoin transaction with a UNIX time stamp entry encoded in the raw hex.
    /// This is used by POS coins
    /// </summary>
    public class TransactionSerializerReddcoin : TransactionSerializer
    {
        public TransactionSerializerReddcoin(CoinParameters parameters)
            : base(parameters)
        {
        }

        protected override void ReadLocktime(BinaryReader reader, Transaction transaction)
        {
            base.ReadLocktime(reader, transaction);

            // RDD writes the timestamp after the lock time
            transaction.Timestamp = (int)BitHelper.ToUInt32(reader.ReadBytes(4));
        }

        protected override void WriteLocktime(BinaryWriter writer, Transaction transaction)
        {
            base.WriteLocktime(writer, transaction);

            // RDD writes the timestamp after the lock time
            writer.Write(Convert.ToUInt32(transaction.Timestamp));
        }

        protected override void WriteSigHash(BinaryWriter writer, Transaction transactionm, Transaction.SigHash sigHash)
        {
            if (sigHash != Transaction.SigHash.Undefined)
            {
                // this is a signing transaction so we need to remove the timestamp
                writer.Seek(-4, SeekOrigin.Current);
                base.WriteSigHash(writer, transactionm, sigHash);
            }
        }
    }
}
