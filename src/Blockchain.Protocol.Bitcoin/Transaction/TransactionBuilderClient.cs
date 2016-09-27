// <copyright file="TransactionBuilderClient.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Transaction
{
    #region Using Directives

    using System;
    using System.Threading.Tasks;

    using Blockchain.Protocol.Bitcoin.Client;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    #endregion

    /// <summary>
    /// Remote transaction builder using the client.
    /// </summary>
    public class TransactionBuilderClient : TransactionBuilder
    {
        /// <summary>
        /// Build a transacting using the coins client.
        /// This will communicate with the client over https.
        /// </summary>
        public override Task Build(IBitcoinClient client, TransactionContext transactionContext)
        {
            // this code should not be use in production as clients are not ssl enabled (no need for that)
            // to use in test scenarios delete this exception
            throw new NotImplementedException();

            ////// create the transaction hex
            ////transactionContext.UnsignedRawTransaction = client.CreateRawTransactionAsync(transactionContext.CreateRawTransaction).Result;
            ////transactionContext.UnsignedTransaction = client.DecodeRawTransactionAsync(transactionContext.UnsignedRawTransaction).Result;

            ////// create transaction signature.
            ////transactionContext.SignRawTransaction = new SignRawTransaction(transactionContext.UnsignedRawTransaction);
            ////transactionContext.SendItems.Where(m => !m.Failed).ForEach(a => a.SpendFromAddresses.ForEach(m => transactionContext.SignRawTransaction.AddKey(m.PrivateKey)));

            ////// sign the transaction.
            ////transactionContext.SignedRawTransaction = client.SignRawTransactionAsync(transactionContext.SignRawTransaction).Result;
            ////transactionContext.DecodedRawTransaction = client.DecodeRawTransactionAsync(transactionContext.SignedRawTransaction.Hex).Result;

            ////return Task.FromResult(0);
        }
    }
}
