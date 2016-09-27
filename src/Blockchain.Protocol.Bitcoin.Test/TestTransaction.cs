// <copyright file="TestTransaction.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Client;
    using Blockchain.Protocol.Bitcoin.Client.Types;
    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;
    using Blockchain.Protocol.Bitcoin.Transaction;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rosetta.Core.Serialization;

    using TransactionInfo = Blockchain.Protocol.Bitcoin.Transaction.Types.TransactionInfo;

    /// <summary>
    /// The transaction.
    /// </summary>
    [TestClass]
    public class TestTransaction
    {
        [TestMethod]
        public void Given_an_unsigned_timestamped_transaction_with_multipul_inputs_When_serialized_to_hex_Then_is_valid()
        {
            var param = new CoinParameters { CoinTag = "PPC", PublicKeyAddressVersion = 55, PrivateKeyVersion = 183 };
            var builder = TransactionBuilder.Create(param);

            var create = new CreateRawTransaction()
            .AddInput("c41e47d99557b2c25e72ee3c6bacf9796b09791cda5ea654225621701a2bc87d", 1)
            .AddInput("32b137d72a26681fa4ac1de3cab1cdcf05e0fd592b0a1cd81a3bd35000dfddf4", 1)
            .AddInput("69a3b97a4207eb7da766b9a29862acae00d3f3e00965d3b31f879abae1a58a50", 0)
            .AddOutput("PXCfHLaR2RCMnHsYCAtSoyjwBkhaniC9Ty", 1.10000000M)
            .AddOutput("PXDHGy2j11atyPoqyNZfccwS46gv6K4BkL", 20.82399100M)
            .AddOutput("PEBAEAM6R7SZkhQmvHa7eBfCNfTCE5CGYT", 1.40000000M)
            .AddOutput("PFj9YzkCRiihArX46KM9kZappnWa5svFhe", 2.53000000M);
            var trx = builder.CreatePubKeyHashTransaction(param, create);
            trx.Timestamp = 1461838729; // specific to this transaction
            var unsigned = builder.Serializer.ToHex(trx);

            Assert.AreEqual("0100000089e32157037dc82b1a7021562254a65eda1c79096b79f9ac6b3cee725ec2b25795d9471ec40100000000fffffffff4dddf0050d33b1ad81c0a2b59fde005cfcdb1cae31daca41f68262ad737b1320100000000ffffffff508aa5e1ba9a871fb3d36509e0f3d300aeac6298a2b966a77deb07427ab9a3690000000000ffffffff04e0c81000000000001976a914f808ce1d1a2c111581237f65931e8ae56d6cbfd088acb7bf3d01000000001976a914f826d9dfd3b0dfdacf09ea6cc7d48c9569ace1b288acc05c1500000000001976a9143d45ff4529a9326804aa1fe2800e55418c00604c88acd09a2600000000001976a9144e4adeb9f14731d0370b10cff700e12c36571b7e88ac00000000", unsigned);
        }

        [TestMethod]
        public void Given_a_signed_timestamped_transaction_with_multipul_inputs_When_serialized_to_hex_Then_is_valid()
        {
            var param = new CoinParameters { CoinTag = "PPC", PublicKeyAddressVersion = 55, PrivateKeyVersion = 183 };
            var builder = TransactionBuilder.Create(param);

            var key1 = new BitcoinPrivateKey(param, "79rzXNEtHfWJQxPUHJpMLnuuLKD9NnJAFeYVYtus2JjyXcKkCQ1");
            var key2 = new BitcoinPrivateKey(param, "7Ab1bFhaW5oC1LN7AgaHKr9ZZGumLSxThq8qMP3iP1oJYwAW2Kq");

            var create = new CreateRawTransaction()
            .AddInput("c41e47d99557b2c25e72ee3c6bacf9796b09791cda5ea654225621701a2bc87d", 1)
            .AddInput("32b137d72a26681fa4ac1de3cab1cdcf05e0fd592b0a1cd81a3bd35000dfddf4", 1)
            .AddInput("69a3b97a4207eb7da766b9a29862acae00d3f3e00965d3b31f879abae1a58a50", 0)
            .AddOutput("PXCfHLaR2RCMnHsYCAtSoyjwBkhaniC9Ty", 1.10000000M)
            .AddOutput("PXDHGy2j11atyPoqyNZfccwS46gv6K4BkL", 20.82399100M)
            .AddOutput("PEBAEAM6R7SZkhQmvHa7eBfCNfTCE5CGYT", 1.40000000M)
            .AddOutput("PFj9YzkCRiihArX46KM9kZappnWa5svFhe", 2.53000000M);
            var trx = builder.CreatePubKeyHashTransaction(param, create);
            trx.Timestamp = 1461838729; // specific to this transaction
            var unsigned = builder.Serializer.ToHex(trx);

            var sign = builder.CreateSignature(unsigned)
            .AddKey(key1.ToWifKey(param))
            .AddKey(key2.ToWifKey(param))
            .AddInput("c41e47d99557b2c25e72ee3c6bacf9796b09791cda5ea654225621701a2bc87d", 1, 
                       "76a914f826d9dfd3b0dfdacf09ea6cc7d48c9569ace1b288ac")
            .AddInput("32b137d72a26681fa4ac1de3cab1cdcf05e0fd592b0a1cd81a3bd35000dfddf4", 1, 
                       "76a9144e4adeb9f14731d0370b10cff700e12c36571b7e88ac")
            .AddInput("69a3b97a4207eb7da766b9a29862acae00d3f3e00965d3b31f879abae1a58a50", 0, 
                       "76a9144e4adeb9f14731d0370b10cff700e12c36571b7e88ac");

            trx = builder.Sign(param, sign);
            var signed = builder.Serializer.ToHex(trx);
            Assert.AreEqual("0100000089e32157037dc82b1a7021562254a65eda1c79096b79f9ac6b3cee725ec2b25795d9471ec4010000008a47304402200cf6263646697a3e90114ead2d461c56a26cb22c3758c1aa3bdd4d1e2d8461ba022048d1c17cc00ab21177a14bcac61fcc990969e1b855c1cdc9ede503af56d924410141042cfdbfaee53942b2c8d9c67631e099e604e6e4e01594394942f33fa237eca310ac0aa79189d96c79e9e764826243614ebdfd400680421dff54314d9eabb87daafffffffff4dddf0050d33b1ad81c0a2b59fde005cfcdb1cae31daca41f68262ad737b132010000008b483045022100c867962f5c584897e5231e45d06065e4bc9dbf26ea4bcbc6de3ceebe87791ade02205c8b016d84a38a25275d89da8f6435016313e1d108f15638609e791570fbd8a10141040c65817203cc55de4c2ab32413497fd9e917a5c1d4711e7772a7277f491d3dc50879852e697a2551547c5d8e28a0a97280e8e63d4590a9c722ce2a84c6d71f3affffffff508aa5e1ba9a871fb3d36509e0f3d300aeac6298a2b966a77deb07427ab9a369000000008b4830450221008d55d7eb528622f4d803c0756f11e49fd6b3206789d6603e9f3221a3473cda18022002663afafc52cc4c3a5bc9c16401d13a853f2e0b4b258c923ec3c18e85aeab100141040c65817203cc55de4c2ab32413497fd9e917a5c1d4711e7772a7277f491d3dc50879852e697a2551547c5d8e28a0a97280e8e63d4590a9c722ce2a84c6d71f3affffffff04e0c81000000000001976a914f808ce1d1a2c111581237f65931e8ae56d6cbfd088acb7bf3d01000000001976a914f826d9dfd3b0dfdacf09ea6cc7d48c9569ace1b288acc05c1500000000001976a9143d45ff4529a9326804aa1fe2800e55418c00604c88acd09a2600000000001976a9144e4adeb9f14731d0370b10cff700e12c36571b7e88ac00000000", signed);
        }

        [TestMethod]
        public void Given_an_unsigned_transaction_with_multipul_inputs_When_serialized_to_hex_Then_is_valid()
        {
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);

            var create = new CreateRawTransaction()
            .AddInput("2a99b070f457c8f2f6cb0f818b951c2038759af7abd98e57aecaf73989cd3474", 1)
            .AddInput("2a99b070f457c8f2f6cb0f818b951c2038759af7abd98e57aecaf73989cd3474", 0)
            .AddOutput("19vrUuKgwKvAhiXfwVBaeHDQo7wMJg4VjD", 0.00020000M)
            .AddOutput("1227pgTWYNsPNVN647NzsceFeXeLt18fTT", 0.00005000M);
            var trx = builder.CreatePubKeyHashTransaction(param, create);
            var unsigned = builder.Serializer.ToHex(trx);

            Assert.AreEqual("01000000027434cd8939f7caae578ed9abf79a7538201c958b810fcbf6f2c857f470b0992a0100000000ffffffff7434cd8939f7caae578ed9abf79a7538201c958b810fcbf6f2c857f470b0992a0000000000ffffffff02204e0000000000001976a91461f05b61eb47be1d96e417272d3d3d3c346e2e9188ac88130000000000001976a9140b2e3c8cfae581f5fa526070a705e617d3c61be488ac00000000", unsigned);
        }

        [TestMethod]
        public void Given_a_signed_transaction_with_multipul_inputs_When_serialized_to_hex_Then_is_valid()
        {
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);

            var key1 = new BitcoinPrivateKey(param, "KxSFAwxgg1kh2CKqmVGDJfQBKypAabiJ7UgDbJcvnRQ3ccbCm8oz");
            var key2 = new BitcoinPrivateKey(param, "KwgcwLEWhzpcWvZtEJuYiPk4BJHNtGhp1tdqL2wKt1xe4Kv8kVd5");

            var create = builder.CreateTransaction()
            .AddInput("2a99b070f457c8f2f6cb0f818b951c2038759af7abd98e57aecaf73989cd3474", 1)
            .AddInput("2a99b070f457c8f2f6cb0f818b951c2038759af7abd98e57aecaf73989cd3474", 0)
            .AddOutput("19vrUuKgwKvAhiXfwVBaeHDQo7wMJg4VjD", 0.00020000M)
            .AddOutput("1227pgTWYNsPNVN647NzsceFeXeLt18fTT", 0.00005000M);
            var trx = builder.CreatePubKeyHashTransaction(param, create);
            var unsigned = builder.Serializer.ToHex(trx);

            var sign = builder.CreateSignature(unsigned)
            .AddKey(key1.ToWifKey(param))
            .AddKey(key2.ToWifKey(param))
            .AddInput("2a99b070f457c8f2f6cb0f818b951c2038759af7abd98e57aecaf73989cd3474", 0, 
                       "76a914a92c9ee052cdcb7ac955b179d5d5553aa1aed28f88ac")
            .AddInput("2a99b070f457c8f2f6cb0f818b951c2038759af7abd98e57aecaf73989cd3474", 1, 
                       "76a914c789d0b3ec2bee3b2e057e2b759d1bd249095cd888ac");

            trx = builder.Sign(param, sign);
            var signed = builder.Serializer.ToHex(trx);
            Assert.AreEqual("01000000027434cd8939f7caae578ed9abf79a7538201c958b810fcbf6f2c857f470b0992a010000006b483045022100b8642683528bce3b71911e970b045c05309035e410c7d7b3ff4def78958fe8670220148eb4bf6b07aa32b74c1041dc1c98b6646d68c3f4001b7e7683abd705ec4c3101210396c9b0b5664c28c4667e9931a9665a7d99222794b3a1d514e3dd198c2cc688e0ffffffff7434cd8939f7caae578ed9abf79a7538201c958b810fcbf6f2c857f470b0992a000000006b48304502210083099361f7feca8003687bb1a5454447846adec11b23e9760cc0c96273fcd39202207894c09e8744b266dfac87dcf0e6aedb848caf4aa451b285a14dc6fc041b6a1a012102b5d6110585425f5af0eaf84262e3962477d7974f97ed5ead5bd25a1eb415c40dffffffff02204e0000000000001976a91461f05b61eb47be1d96e417272d3d3d3c346e2e9188ac88130000000000001976a9140b2e3c8cfae581f5fa526070a705e617d3c61be488ac00000000", signed);
        }

        [TestMethod]
        public void Given_an_unsigned_transaction_When_serialized_to_hex_Then_is_valid()
        {
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);

            var create = new CreateRawTransaction()
            .AddInput("9e779dbc84fe9c4082bc6fff332ddee81a6f56908720f25cb83ea9441cfa801b", 0)
            .AddOutput("19vrUuKgwKvAhiXfwVBaeHDQo7wMJg4VjD", 0.00010000M)
            .AddOutput("1KC4ZkESbdjYVfdYWuAkmSscQfD1VEWb32", 0.00030000M);
            var trx = builder.CreatePubKeyHashTransaction(param, create);
            var unsigned = builder.Serializer.ToHex(trx);

            Assert.AreEqual("01000000011b80fa1c44a93eb85cf2208790566f1ae8de2d33ff6fbc82409cfe84bc9d779e0000000000ffffffff0210270000000000001976a91461f05b61eb47be1d96e417272d3d3d3c346e2e9188ac30750000000000001976a914c789d0b3ec2bee3b2e057e2b759d1bd249095cd888ac00000000", unsigned);
        }

        [TestMethod]
        public void Given_a_signed_transaction_When_serialized_to_hex_Then_is_valid()
        {
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);

            var prv = "KxSFAwxgg1kh2CKqmVGDJfQBKypAabiJ7UgDbJcvnRQ3ccbCm8oz";
            var key = new BitcoinPrivateKey(param, prv);
            var destination = Address.Create(param, "19vrUuKgwKvAhiXfwVBaeHDQo7wMJg4VjD");
            var change = Address.Create(param, "1KC4ZkESbdjYVfdYWuAkmSscQfD1VEWb32");
            
            var create = new CreateRawTransaction()
            .AddInput("9e779dbc84fe9c4082bc6fff332ddee81a6f56908720f25cb83ea9441cfa801b", 0)
            .AddOutput(destination.ToString(), 0.00010000M)
            .AddOutput(change.ToString(), 0.00030000M);
            var trx = builder.CreatePubKeyHashTransaction(param, create);
            var unsigned = builder.Serializer.ToHex(trx);

            var sign = new SignRawTransaction(unsigned)
            .AddKey(key.ToWifKey(param))
            .AddInput("9e779dbc84fe9c4082bc6fff332ddee81a6f56908720f25cb83ea9441cfa801b", 
                       0, 
                       "76a914c789d0b3ec2bee3b2e057e2b759d1bd249095cd888ac");

            trx = builder.Sign(param, sign);
            var signed = builder.Serializer.ToHex(trx);
            Assert.AreEqual("01000000011b80fa1c44a93eb85cf2208790566f1ae8de2d33ff6fbc82409cfe84bc9d779e000000006b483045022100911e4ffada078f43c792b797b0d864a519cb088083b89003810aaf28325a6ecd02207f2e9790511710ae93309f4b3b30c0379b8917b6a0fb1cdcc9c711fc6f1831ef01210396c9b0b5664c28c4667e9931a9665a7d99222794b3a1d514e3dd198c2cc688e0ffffffff0210270000000000001976a91461f05b61eb47be1d96e417272d3d3d3c346e2e9188ac30750000000000001976a914c789d0b3ec2bee3b2e057e2b759d1bd249095cd888ac00000000", signed);
        }

        [TestMethod]
        public void Given_an_rpc_transaction_When_deserialized_from_hex_Then_is_successfully_compared()
        {
            // A json transaction as it's returned by the rpc method
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);
            var jsonTransaction = File.OpenText(@"..\..\Resources\Transaction.json").ReadToEnd();
            var trx = JsonSerializer.DeSerialize<DecodedRawTransaction>(jsonTransaction);

            var decoded = builder.Serializer.FromHex(trx.Hex);
            CompareTransactions(decoded, builder.Parse(trx));
        }

        [TestMethod]
        public void Given_an_rpc_transaction_When_deserialized_to_hex_Then_is_successfully_compared()
        {
            // A json transaction as it's returned by the rpc method
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);
            var jsonTransaction = File.OpenText(@"..\..\Resources\Transaction.json").ReadToEnd();
            var trx = JsonSerializer.DeSerialize<DecodedRawTransaction>(jsonTransaction);
            var rawTransaction = builder.Serializer.ToHex(builder.Parse(trx));

            Assert.AreEqual(trx.Hex, rawTransaction);
        }

        [TestMethod]
        public void Given_an_rpc_coinbase_transaction_When_deserialized_from_hex_Then_is_successfully_compared()
        {
            // A json transaction as it's returned by the rpc method
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);
            var jsonCoinbaseTransaction = File.OpenText(@"..\..\Resources\CoinbaseTransaction.json").ReadToEnd();
            var trx = JsonSerializer.DeSerialize<DecodedRawTransaction>(jsonCoinbaseTransaction);

            var decoded = builder.Serializer.FromHex(trx.Hex);
            CompareTransactions(decoded, builder.Parse(trx));
        }

        [TestMethod]
        public void Given_an_rpc_coinbase_transaction_When_deserialized_to_hex_Then_is_successfully_compared()
        {
            // A json transaction as it's returned by the rpc method
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);
            var jsonCoinbaseTransaction = File.OpenText(@"..\..\Resources\CoinbaseTransaction.json").ReadToEnd();
            var trx = JsonSerializer.DeSerialize<DecodedRawTransaction>(jsonCoinbaseTransaction);
            var rawTransaction = builder.Serializer.ToHex(builder.Parse(trx));

            Assert.AreEqual(trx.Hex, rawTransaction);
        }

        [TestMethod]
        public void Given_an_rpc_transaction_with_big_inputs_When_deserialized_from_hex_Then_is_successfully_compared()
        {
            // A json transaction as it's returned by the rpc method
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);
            var bigTransaction = File.OpenText(@"..\..\Resources\BigTransaction.json").ReadToEnd();
            var trx = JsonSerializer.DeSerialize<DecodedRawTransaction>(bigTransaction);

            var decoded = builder.Serializer.FromHex(trx.Hex);
            CompareTransactions(decoded, builder.Parse(trx));
        }

        [TestMethod]
        public void Given_an_rpc_transaction_with_big_inputs_When_deserialized_to_hex_Then_is_successfully_compared()
        {
            // A json transaction as it's returned by the rpc method
            var param = new CoinParameters { CoinTag = "BTC", PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };
            var builder = TransactionBuilder.Create(param);
            var BigTransaction = File.OpenText(@"..\..\Resources\BigTransaction.json").ReadToEnd();
            var trx = JsonSerializer.DeSerialize<DecodedRawTransaction>(BigTransaction);
            var rawTransaction = builder.Serializer.ToHex(builder.Parse(trx));

            Assert.AreEqual(trx.Hex, rawTransaction);
        }

        public static void CompareTransactions(Transaction source, Transaction target)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(target);

            Assert.AreEqual(source.Hash, target.Hash);
            Assert.AreEqual(source.Version, target.Version);
            Assert.AreEqual(source.Locktime, target.Locktime);

            Assert.AreEqual(source.Inputs.Count(), target.Inputs.Count());
            foreach (var vin in Enumerable.Range(0, source.Inputs.Count()))
            {
                if (CryptoUtil.ConvertHex(source.Inputs.ElementAt(vin).Outpoint.Hash).SequenceEqual(CryptoUtil.ZeroHash(32)))
                {
                    Assert.IsTrue(source.Inputs.ElementAt(vin).ScriptBytes.SequenceEqual(target.Inputs.ElementAt(vin).ScriptBytes));
                }
                else
                {
                    Assert.AreEqual(source.Inputs.ElementAt(vin).Outpoint.Hash, target.Inputs.ElementAt(vin).Outpoint.Hash);
                    Assert.AreEqual(source.Inputs.ElementAt(vin).Outpoint.Index, target.Inputs.ElementAt(vin).Outpoint.Index);
                    Assert.IsTrue(source.Inputs.ElementAt(vin).ScriptBytes.SequenceEqual(target.Inputs.ElementAt(vin).ScriptBytes));
                }
            }
            
            Assert.AreEqual(source.Outputs.Count(), target.Outputs.Count());
            foreach (var vout in Enumerable.Range(0, source.Outputs.Count()))
            {
                Assert.AreEqual(source.Outputs.ElementAt(vout).Index, target.Outputs.ElementAt(vout).Index);
                Assert.AreEqual(source.Outputs.ElementAt(vout).Value, target.Outputs.ElementAt(vout).Value);
                Assert.IsTrue(source.Outputs.ElementAt(vout).ScriptBytes.SequenceEqual(target.Outputs.ElementAt(vout).ScriptBytes));
            }
        }
    }
}
