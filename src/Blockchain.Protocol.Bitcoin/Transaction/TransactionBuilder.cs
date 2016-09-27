// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionBuilder.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Transaction
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Client;
    using Blockchain.Protocol.Bitcoin.Client.Types;
    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;
    using Blockchain.Protocol.Bitcoin.Transaction.Script;
    using Blockchain.Protocol.Bitcoin.Transaction.Serializers;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    #endregion

    /// <summary>
    /// The transaction builder.
    /// </summary>
    public class TransactionBuilder
    {
        protected TransactionBuilder()
        {
        }

        protected TransactionBuilder(CoinParameters param, TransactionSerializer serializer, TransactionSigner signer)
        {
            this.CoinParameters = param;
            this.Serializer = serializer;
            this.Signer = signer;
        }

        public CoinParameters CoinParameters { get; set; }

        public TransactionSerializer Serializer { get; set; }

        public TransactionSigner Signer { get; set; }

        /// <summary>
        /// Create a transaction builder for a given coin.
        /// Some other Blockchain coins have different serializes of network parameters, 
        /// this is calculated when creating the builder.
        /// </summary>
        public static TransactionBuilder Create(CoinParameters param)
        {
            AddressUtil.PopulateCoinParameters(param);

            // todo: move factory selector in a configuration file
            var lst = new List<string>
                          {
                              "BTC", "TRC", "GRC", "DOGE", "DASH", "RDD", "XPM", "LTC", "NMC", 
                              "QRK", "PPC", "MTR", "GB", "SHM", "CRX", "UBIQ", "ARG", "ZYD", "DLC",
                              "STRAT", "SH"
                          };

            if (lst.Contains(param.CoinTag))
            {
                var ser = new TransactionSerializer(param);
                // coin scale can be found in util.h (static const int64_t COIN = 100000000)
                param.CoinScale = 100000000;
                param.TransactionVersion = 1;

                if (param.CoinTag == "QRK")
                {
                    param.CoinScale = 100000;
                }

                if (param.CoinTag == "PPC")
                {
                    param.CoinScale = 1000000;
                    ser = new TransactionSerializerTimeStamped(param);
                }
                
                if (param.CoinTag == "GRC")
                {
                    ser = new TransactionSerializerGridcoin(param);
                }

                if (param.CoinTag == "RDD")
                {
                    param.TransactionVersion = 2;
                    ser = new TransactionSerializerReddcoin(param);
                }

                if (param.CoinTag == "MTR")
                {
                    ser = new TransactionSerializerTimeStamped(param);
                }

                if (param.CoinTag == "GB")
                {
                    ser = new TransactionSerializerTimeStamped(param);
                }

                if (param.CoinTag == "SHM")
                {
                    ser = new TransactionSerializerTimeStamped(param);
                }

                if (param.CoinTag == "CRX")
                {
                    ser = new TransactionSerializerTimeStamped(param);
                }

                if (param.CoinTag == "UBIQ")
                {
                    ser = new TransactionSerializerTimeStamped(param);
                }

                if (param.CoinTag == "STRAT")
                {
                    ser = new TransactionSerializerTimeStamped(param);
                }

                var builder = new TransactionBuilder(param, ser, new TransactionSigner(param, ser));

                return builder;
            }

            return new TransactionBuilderClient();
        }

        /// <summary>
        /// Build a transaction.
        /// </summary>
        public virtual Task Build(IBitcoinClient client, TransactionContext transactionContext)
        {
            var transaction = this.CreatePubKeyHashTransaction(this.CoinParameters, transactionContext.CreateRawTransaction);

            // create the transaction hex
            transactionContext.UnsignedRawTransaction = this.Serializer.ToHex(transaction);
            ////var createdHex = await client.CreateRawTransactionAsync(transactionContext.CreateRawTransaction);
            ////var created = this.Serializer.FromHex(createdHex);

            // validate
            ////transactionContext.UnsignedTransaction = await client.DecodeRawTransactionAsync(transactionContext.UnsignedRawTransaction);

            // create transaction signature and add the relevant private keys and previous outputs.
            transactionContext.SignRawTransaction = new SignRawTransaction(transactionContext.UnsignedRawTransaction);
            
            //transactionContext.SendItems.Where(m => !m.Failed).ForEach(m => transactionContext.SignRawTransaction.AddKey(m.SpendFromAddress.PrivateKey));
            //transactionContext.PreviousRawTransactions.ForEach(p => p.VOut.ForEach(output => transactionContext.SignRawTransaction.AddInput(p.TxId, output.N, output.ScriptPubKey.Hex)));
            transactionContext.SendItems
                .Where(m => !m.Failed)
                .ForEach(m => 
                    m.SpendFromAddresses.ForEach(b =>
                        {
                            transactionContext.SignRawTransaction.AddKey(b.PrivateKey);
                            b.Transactions.ForEach(a => transactionContext.SignRawTransaction.AddInput(a.Hash, (int)a.Index, a.ScriptPubKeyHex));
                        }));

            // ready to sign
            transaction = this.Sign(this.CoinParameters, transactionContext.SignRawTransaction);
            transactionContext.SignedRawTransaction = new SignedRawTransaction { Hex = this.Serializer.ToHex(transaction), Complete = true };

            ////var signed = await client.SignRawTransactionAsync(transactionContext.SignRawTransaction);

            // validate the transaction
            //transactionContext.DecodedRawTransaction = await client.DecodeRawTransactionAsync(transactionContext.SignedRawTransaction.Hex);

            return Task.FromResult(true);
        }

        /// <summary>
        /// The create a pay to pub key hash transaction.
        /// </summary>
        public Transaction CreatePubKeyHashTransaction(CoinParameters parameters, CreateRawTransaction rawTransaction)
        {
            var transaction = new Transaction
            {
                Version = parameters.TransactionVersion, 
                Locktime = 0,
                Timestamp = (int)DateTime.UtcNow.UnixTimeStampFromDateTime()
            };

            // create the inputs
            transaction.Inputs = rawTransaction.Inputs
                .Select(input => new TransactionInput
                {
                    Outpoint = new TransactionOutPoint
                    {
                        Hash = input.TransactionId, 
                        Index = input.Output
                    }, 
                    ScriptBytes = Enumerable.Empty<byte>().ToArray(), 

                    Sequence = 0xffffffff // uint.MaxValue
                }).ToList();

            // create the output
            transaction.Outputs = rawTransaction.Outputs
                .Select((output, index) => new TransactionOutput
                {
                    Index = index, 
                    Value = this.Serializer.ValueToNum(output.Value), 
                    ScriptBytes = ScriptBuilder.CreateOutputScript(Address.Create(parameters, output.Key)).GetProgram()
                }).ToList();

            return transaction;
        }

        /// <summary>
        /// Create the signing bag with matching scripts and private keys
        /// Then sign the inputs.
        /// </summary>
        public Transaction Sign(CoinParameters parameters, SignRawTransaction rawTransactions)
        {
            var transaction = this.Serializer.FromHex(rawTransactions.RawTransactionHex);
            var bag = new TransactionSigner.SignerBag { Items = new List<TransactionSigner.RedeemScript>() };
            var keys = rawTransactions.PrivateKeys.Select(s => new BitcoinPrivateKey(parameters, s)).ToList();

            // create a linked object between the pub key hash and its outpoint
            var inputs = rawTransactions.Inputs
                .Select(input => new { Input = input, PubKeyHash = new Script.Script(CryptoUtil.ConvertHex(input.ScriptPubKey)).TryGetPubKeyHash() })
                .Where(p => p.PubKeyHash.IsNotNull()).ToList();

            // compare private keys with redeem script pub key hash and add to the bag
            foreach (var key in keys)
            {
                // there should be at least one redeem script per private key
                var inserts = inputs.Where(f => f.PubKeyHash.SequenceEqual(key.Hash160)).ToList();
                Thrower.If(inserts.None()).Throw<TransactionException>("Private key had no matching redeem script '{0}'".StringFormat(key.PublicKey.ToAddress(parameters).ToString()));
                inserts.ForEach(insert => bag.Add(insert.Input.TransactionId, insert.Input.Output, insert.Input.ScriptPubKey, key));
            }
            
            this.Signer.SignInputs(transaction, bag);

            return transaction;
        }

        public Transaction Parse(DecodedRawTransaction source)
        {
            return new Transaction
            {
                Hash = source.TxId,
                Locktime = source.Locktime,
                Timestamp = source.Time,
                Version = source.Version,
                Inputs = source.VIn.Select(vin => new TransactionInput
                {
                    Outpoint = new TransactionOutPoint
                    {
                        Hash = vin.CoinBase.IsNull() ? vin.TxId : CryptoUtil.ToHex(CryptoUtil.ZeroHash(32)),
                        Index = vin.CoinBase.IsNull() ? vin.VOut : (long)uint.MaxValue
                    },
                    ScriptBytes = CryptoUtil.ConvertHex(vin.CoinBase.IsNull() ? vin.ScriptSig.Hex : vin.CoinBase),
                    Sequence = vin.Sequence
                }).ToList(),
                Outputs = source.VOut.Select(vout => new TransactionOutput
                {
                    Index = vout.N,
                    Value = this.Serializer.ValueToNum(vout.Value),
                    ScriptBytes = vout.ScriptPubKey.Hex.IsNullOrEmpty() ? Enumerable.Empty<byte>().ToArray() : CryptoUtil.ConvertHex(vout.ScriptPubKey.Hex)
                }).ToList()
            };
        }

        /// <summary>
        /// The create an unsigned transaction.
        /// </summary>
        public CreateRawTransaction CreateTransaction()
        {
            return new CreateRawTransaction();
        }

        /// <summary>
        /// Sign a transaction.
        /// </summary>
        public SignRawTransaction CreateSignature(string rawTrx)
        {
            return new SignRawTransaction(rawTrx);
        }
    }
}
