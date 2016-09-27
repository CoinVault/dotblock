// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionManager.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Transaction
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Blockchain.Protocol.Bitcoin.Client;
    using Blockchain.Protocol.Bitcoin.Client.Types;
    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    using TransactionInfo = Blockchain.Protocol.Bitcoin.Transaction.Types.TransactionInfo;

    #endregion

    /// <summary>
    /// The transaction utility.
    /// </summary>
    public class TransactionManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///  Create a transaction.
        /// This allows to create a transaction using the private keys, 
        /// This means we don't need to open the wallet to send the transaction and all the handling of open wallet synchronization is not required.
        /// </summary>
        public static async Task<TransactionContext> CreateAsync(CoinParameters param, IBitcoinClient client, IEnumerable<TransactionInfo> items)
        {
            var context = new TransactionContext
                              {
                                  CoinParameters = param, 
                                  PreviousRawTransactions = new List<DecodedRawTransaction>(), 
                                  CreateRawTransaction = new CreateRawTransaction(), 
                                  SendItems = new List<TransactionInfo>()
                              };

            foreach (var item in items)
            {
                context.SendItems.Add(item);

                // check if receiving address is already in the transaction, this is allowed but not supported by the client.
                if (item.SpendToAddresses.Select(s => s.PublicKey)
                    .Intersect(item.SpendFromAddresses.Select(s => s.PublicKey)).Any())
                {
                    item.Fail(false, FailedReason.CannotSendToSelfUnlessChange);
                    continue;
                }
                
                if (item.SpendFromAddresses.SelectMany(s => s.Transactions).None())
                {
                    item.Fail(false, FailedReason.NoTransactionsFound);
                    continue;
                }

                // check if a transaction is already in use
                if (context.CreateRawTransaction.Inputs.
                    Any(source => item.SpendFromAddresses.SelectMany(s => s.Transactions)
                        .Any(target => source.TransactionId == target.Hash && source.Output == target.Index)))
                {
                    item.Fail(true, FailedReason.TransactionInputAlreadyInUse);
                    continue;
                }

                // check if receiving address is already in the transaction, this is allowed but not supported by the client.
                if (context.CreateRawTransaction.Outputs
                    .Any(source => item.SpendToAddresses.Any(target => source.Key == target.PublicKey)))
                {
                    item.Fail(true, FailedReason.ReceiveAddressAlreadyInOutput);
                    continue;
                }

                if (item.RequestedFee <= 0)
                {
                    item.Fail(false, FailedReason.FeeCannotBeZero);
                    continue;
                }

                ////try
                ////{
                ////    var rawTrxListAsync = item.SpendFromAddresses
                ////        .SelectMany(s => s.Transactions)
                ////        .DistinctBy(t => t.Hash)
                ////        .Where(a => context.PreviousRawTransactions.None(b => b.TxId == a.Hash))
                ////        .Select(async trx => await client.GetRawTransactionAsync(trx.Hash, 1));

                ////    var previousRawTransactions = (await Task.WhenAll(rawTrxListAsync)).ToList();
                ////    context.PreviousRawTransactions.AddRange(previousRawTransactions);
                 
                ////    // the Gridcoin hack
                ////    GridcoinHack(context.CoinParameters, previousRawTransactions);
                ////}
                ////catch (Exception)
                ////{
                ////    item.Fail(false, FailedReason.FailedReadingTransactionsFromClient);
                ////    continue;
                ////}

                // validate all the transaction outpoints have the ScriptPubKey Hex
                ////item.SpendFromAddresses.ForEach(spend => 
                ////    spend.Transactions.ForEach(strx =>
                ////    {
                ////        var output = context.PreviousRawTransactions
                ////            .Where(t => t.TxId == strx.Hash)
                ////            .SelectMany(s => s.VOut)
                ////            .SingleOrDefault(v => v.N == strx.Index);
                        
                ////        if (output.IsNotNull() && output.Value == strx.Value)
                ////        {
                ////            if (output.ScriptPubKey.IsNotNull())
                ////            {
                ////                if (output.ScriptPubKey.Addresses.Any(addr => addr == spend.PublicKey))
                ////                {
                ////                    strx.ScriptPubKeyHex = output.ScriptPubKey.Hex;
                ////                }
                ////            }
                ////        }
                ////    }));

                if (item.SpendFromAddresses.SelectMany(s => s.Transactions).Any(t => t.ScriptPubKeyHex.IsNullOrEmpty()))
                {
                    item.Fail(false, FailedReason.ScriptPubKeyHexNotFound);
                    continue;
                }

                //// get the outputs associated with the sender address
                var spendToEnumerated = item.SpendToAddresses.ToList();
                var spendFromEnumerated = item.SpendFromAddresses.SelectMany(s => s.Transactions).ToList();
                ////var outputs = previousRawTransactions
                ////    .ToDictionary(rawtrx => rawtrx.TxId, rawtrx => rawtrx.VOut
                ////        .Where(vout => vout.ScriptPubKey.Addresses
                ////            .Any(add => item.SpendFromAddresses.Any(a => a.PublicKey == add))));

                //// calculate the sum of inputs and outputs.
                var inputSum = spendFromEnumerated.Select(t => t.Value).Sum();
                var outputSum = spendToEnumerated.Select(spt => spt.Amount).Sum();

                var change = inputSum - outputSum;

                if (change < 0)
                {
                    item.Fail(false, FailedReason.InvalidSum);
                    continue;
                }
                
                if (item.SpendToAddresses.Any(s => s.TakeFee))
                {
                    //// take the fee from a receiver address.
                    var receiver = item.SpendToAddresses.First(s => s.TakeFee);

                    if (receiver.Amount < item.RequestedFee)
                    {
                        item.Fail(false, FailedReason.InsufficientFeeOnReceiver);
                        continue;
                    }

                    receiver.Amount -= item.RequestedFee;
                }
                else
                {
                    //// take the fee from the sender
                    if (change < item.RequestedFee)
                    {
                        item.Fail(false, FailedReason.InsufficientFeeOnSender);
                        continue;
                    }

                    change -= item.RequestedFee;
                }

                if (change > 0)
                {
                    if (item.ChangeAddress.IsNullOrEmpty())
                    {
                        item.Fail(false, FailedReason.NoChangeAddressFound);
                        continue;
                    }
                }

                // try to calculate the fee if trx is too big the default fee may not be enough
                // http://bitcoin.stackexchange.com/questions/7537/calculator-for-estimated-tx-fees
                // https://en.bitcoin.it/wiki/Transaction_fees
                // http://bitcoin.stackexchange.com/questions/3400/what-is-the-exact-formula-for-calculating-transaction-fees
                // http://bitcoin.stackexchange.com/questions/1195/how-to-calculate-transaction-size-before-sending
                // formula fee = [Normal fee * (500 KB) / (500KB - Block size)] - [Normal fee / (1 - Block size / 500KB)]
                // trx size = [in*148 + out*34 + 10 plus or minus 'in']

                //// TODO: move this code to a facotry to allow calculation of fee per coin
                //// coins have different block sizes sand this needs to be acocunted for as well
                
                var ins = spendFromEnumerated.Count();
                var outs = spendToEnumerated.Count();
                var sizeByte = Convert.ToDecimal((ins * 148) + (outs * 34) + 10 + (ins / 2));

                // we'll calculate roughly the coin fee times size of transaction
                const decimal AvgSizeBytes = 1000;
                if (sizeByte >= AvgSizeBytes)
                {
                    var feeRatio = sizeByte / AvgSizeBytes;
                    var expectedFee = feeRatio * item.CoinFee;

                    if (expectedFee > item.RequestedFee)
                    {
                        item.Fail(false, FailedReason.InsufficientFeeForTransactionSize);
                        continue;
                    }
                }

                spendFromEnumerated.ForEach(vout => context.CreateRawTransaction.AddInput(vout.Hash, (int)vout.Index));
                spendToEnumerated.ForEach(spt => context.CreateRawTransaction.AddOutput(spt.PublicKey, spt.Amount));                

                if (change > 0)
                {
                    context.CreateRawTransaction.AddOutput(item.ChangeAddress, change);
                }
            }

            if (context.SendItems.All(w => w.Failed))
            {
                return context;
            }

            //// create the builder and build the transaction (either a client using the coins client or locally if supported).
            await TransactionBuilder.Create(param).Build(client, context);

            // seems we are done here
            return context;
        }

        /// <summary>
        /// This method does some final operation on the context
        /// </summary>
        /// <param name="coinParameters">The coinParameters.</param>
        /// <param name="previousRawTransactions">The transaction to modify.</param>
        private static void GridcoinHack(CoinParameters coinParameters, List<DecodedRawTransaction> previousRawTransactions)
        {
            if (coinParameters.CoinTag == "GRC")
            {
                //// for some reason the GRC client will not return the HEX of an output
                //// pars the raw transaction and populate the HEX from there
                //// this will be fixed in the next release of the Gridcoin client
                //// see git issue and pull request https://github.com/gridcoin/Gridcoin-Research/issues/86#issuecomment-218024574

                previousRawTransactions.ForEach(trx =>
                    {
                        var cloned = TransactionBuilder.Create(coinParameters).Serializer.FromHex(trx.Hex);

                        trx.VOut.ForEach(output =>
                            {
                                output.ScriptPubKey.Hex = CryptoUtil.ToHex(cloned.Outputs.ElementAt(output.N).ScriptBytes);
                            });
                    });
            }
        }

        #endregion
    }
}
