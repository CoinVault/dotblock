// <copyright file="Transaction.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Transaction.Types
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public class Transaction 
    {
        /// <summary>
        /// The signature hash for anyone can pay.
        /// </summary>
        public static byte SighashAnyonecanpayValue = 0x80;

        /// <summary>
        /// The sig hash.
        /// </summary>
        public enum SigHash : uint
        {
            /// <summary>
            /// Undefined signature hash.
            /// </summary>
            Undefined = 0, 

            /// <summary>
            /// All outputs are signed
            /// </summary>
            All = 1, 

            /// <summary>
            /// No outputs as signed
            /// </summary>
            None = 2, 

            /// <summary>
            /// Only the output with the same index as this input is signed
            /// </summary>
            Single = 3, 

            /// <summary>
            /// If set, no inputs, except this, are part of the signature
            /// </summary>
            AnyoneCanPay = 0x80
        }

        public List<TransactionInput> Inputs { get; set; }

        public List<TransactionOutput> Outputs { get; set; }

        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the Timestamp (used by POS coins).
        /// </summary>
        public int Timestamp { get; set; }

        public long Locktime { get; set; }

        public string Hash { get; set; }

        /// <summary>
        /// Clone the transaction, this does not copy over the Script object only the script-bytes.
        /// </summary>
        /// <returns>
        /// The cloned <see cref="Transaction"/>.
        /// </returns>
        public Transaction Clone()
        {
            // todo: add a test for this method.
            return new Transaction
                       {
                           Version = this.Version, 
                           Hash = this.Hash, 
                           Locktime = this.Locktime, 
                           Timestamp = this.Timestamp,
                           Inputs = this.Inputs
                           .Select(vin => new TransactionInput 
                           { 
                               Sequence = vin.Sequence, 
                               Outpoint = new TransactionOutPoint { Hash = vin.Outpoint.Hash, Index = vin.Outpoint.Index }, 
                               ScriptBytes = vin.ScriptBytes.ToArray()
                           })
                           .ToList(), 
                           Outputs = this.Outputs.
                           Select(vout => new TransactionOutput 
                           { 
                               Index = vout.Index, 
                               Value = vout.Value, 
                               ScriptBytes = vout.ScriptBytes.ToArray()
                           })
                           .ToList()
                       };
        }

        public TransactionInput FindInput(TransactionOutPoint outPoint)
        {
            return this.Inputs.First(t => t.Outpoint.Hash == outPoint.Hash && t.Outpoint.Index == outPoint.Index);
        }
    }
}
