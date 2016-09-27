// <copyright file="TransactionSerializerTimeStamped.cs" company="SoftChains">
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
    using System.IO;

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    #endregion

    /// <summary>
    /// Serialize a transaction with a UNIX time stamp entry encoded in the raw hex.
    /// This is used by POS coins like Peercoin and similar
    /// </summary>
    public class TransactionSerializerTimeStamped : TransactionSerializer
    {
        public TransactionSerializerTimeStamped(CoinParameters parameters)
            : base(parameters)
        {
        }

        protected override void ReadTimeStamp(BinaryReader reader, Transaction trx)
        {
            trx.Timestamp = (int)BitHelper.ToUInt32(reader.ReadBytes(4));
        }

        protected override void WriteTimeStamp(BinaryWriter writer, Transaction trx)
        {
            writer.Write(Convert.ToUInt32(trx.Timestamp));
        }
    }
}
