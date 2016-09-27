// <copyright file="TransactionSerializerGridcoin.cs" company="SoftChains">
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

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    #endregion

    /// <summary>
    /// Gridcoin transaction serialize that contains a BOINC Hash
    /// </summary>
    public class TransactionSerializerGridcoin : TransactionSerializerTimeStamped
    {
        public TransactionSerializerGridcoin(CoinParameters parameters)
            : base(parameters)
        {
        }

        public override Transaction FromHex(string rawHexTransaction)
        {
            var trx = base.FromHex(rawHexTransaction);
            
            // the last two char bytes are the BOINC hash (this may be more then two bytes)
            var hashBoinc = rawHexTransaction.Substring(rawHexTransaction.Length - 2);

            return trx;
        }

        public override string ToHex(Transaction transaction, Transaction.SigHash sigHash = Transaction.SigHash.Undefined)
        {
            var hash = base.ToHex(transaction, sigHash);

            if (sigHash == Transaction.SigHash.Undefined)
            {
                // add the hashBoinc to the end of the transaction
                hash += "00";
            }
            else
            {
                // append the hashBoinc before the 4 byte sig hash (8 characters)
                hash = hash.Insert(hash.Length - 8, "00");    
            }

            return hash;
        }
    }
}
