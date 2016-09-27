// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionSigner.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591
namespace Blockchain.Protocol.Bitcoin.Transaction
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;
    using Blockchain.Protocol.Bitcoin.Transaction.Script;
    using Blockchain.Protocol.Bitcoin.Transaction.Serializers;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    #endregion

    /// <summary>
    /// The transaction signature.
    /// </summary>
    public class TransactionSigner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionSigner"/> class.
        /// </summary>
        public TransactionSigner(CoinParameters coinParameters, TransactionSerializer serializer)
        {
            this.CoinParameters = coinParameters;
            this.Serializer = serializer;
        }

        public CoinParameters CoinParameters { get; set; }

        public TransactionSerializer Serializer { get; set; }

        public void SignInputs(Transaction transaction, SignerBag bag)
        {
            foreach (var transactionInput in transaction.Inputs)
            {
                var redeem = bag.Find(transactionInput.Outpoint);
                Thrower.If(redeem.IsNull()).Throw<TransactionException>("redeem script was not found");

                var redeemScript = new Script.Script(CryptoUtil.ConvertHex(redeem.ScriptPubKeyHex));

                var signature = this.CalculateSignature(transaction, transactionInput.Outpoint, redeem.PrivateKey.Key, redeemScript, Transaction.SigHash.All);

                var signedScript = ScriptBuilder.CreateInputScript(signature, redeem.PrivateKey.Key);

                transactionInput.ScriptBytes = signedScript.GetProgram();
            }
        }

        public TransactionSignature CalculateSignature(Transaction transaction, TransactionOutPoint outPoint, EcKey key, Script.Script redeemScript, Transaction.SigHash hashType)
        {
            // at the moment only signing all the outputs is supported
            Thrower.If(hashType != Transaction.SigHash.All).Throw<TransactionException>("Only SigHash type 'All' supported");

            //// clone the transaction and clear all the inputs
            //// only the inputs for the equivalent output needs to be present for signing
            var signTx = transaction.Clone();
            signTx.Inputs.ForEach(input => input.ScriptBytes = Enumerable.Empty<byte>().ToArray());

            // set the redeem script and clear it of 'OP_CODESEPARATOR'
            var connectedScript = redeemScript.GetProgram();
            var redeemConnectedScript = Script.Script.RemoveAllInstancesOfOp(connectedScript, ScriptOpCodes.OP_CODESEPARATOR);

            signTx.FindInput(outPoint).ScriptBytes = redeemConnectedScript;

            // serialize then hash the transaction to HEX and sign it.
            var trxHex = this.Serializer.ToHex(signTx, hashType);
            var hash = CryptoUtil.Sha256HashTwice(CryptoUtil.ConvertHex(trxHex));
            return new TransactionSignature(key.Sign(hash), hashType);
        }

        public class SignerBag
        {
            public List<RedeemScript> Items { get; set; }

            public static SignerBag Create(string hash, long index, string scriptPubKeyHex, BitcoinPrivateKey privateKey)
            {
                return new SignerBag
                {
                    Items = new List<RedeemScript>
                    {
                        new RedeemScript
                        {
                            Hash = hash, 
                            Index = index, 
                            ScriptPubKeyHex = scriptPubKeyHex, 
                            PrivateKey = privateKey
                        }
                    }
                };
            }

            public RedeemScript Find(TransactionOutPoint outPoint)
            {
                return this.Items.FirstOrDefault(t => t.Hash == outPoint.Hash && t.Index == outPoint.Index);
            }

            public RedeemScript Add(string hash, long index, string scriptPubKeyHex, BitcoinPrivateKey privateKey)
            {
                var script = new RedeemScript { Hash = hash, Index = index, ScriptPubKeyHex = scriptPubKeyHex, PrivateKey = privateKey };
                this.Items.Add(script);
                return script;
            }
        }

        public class RedeemScript
        {
            public string Hash { get; set; }

            public long Index { get; set; }

            public string ScriptPubKeyHex { get; set; }

            public BitcoinPrivateKey PrivateKey { get; set; }
        }
    }
}
