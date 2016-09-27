// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestTransaction.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   //   //   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Test
{
    using System.IO;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Client.Types;
    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;
    using Blockchain.Protocol.Bitcoin.Transaction;
    using Blockchain.Protocol.Bitcoin.Transaction.Script;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rosetta.Core.Serialization;

    [TestClass]
    public class TestScript
    {
        private static readonly CoinParameters parameters = new CoinParameters { PublicKeyAddressVersion = 0, PrivateKeyVersion = 128, };

        [TestMethod]
        public void Given_a_scriptsig_When_parsed_Then_should_compare_to_previous_address()
        {
            var jsonTransaction = File.OpenText(@"..\..\Resources\Transaction.json").ReadToEnd();
            var trx = JsonSerializer.DeSerialize<DecodedRawTransaction>(jsonTransaction);

            var sigProgBytes = CryptoUtil.ConvertHex(trx.VIn.First().ScriptSig.Hex);

            ////byte[] sigProgBytes = CryptoUtil.ConvertHex(SigProg);
            var script = new Script(sigProgBytes);

            // Test we can extract the from address.
            var hash160 = AddressUtil.Sha256Hash160(script.GetPubKey());
            Assert.AreEqual("1BYRkxUK1m8fAqYU48AY62rRQpsJc34tX9", Address.Create(parameters, hash160).ToString());
        }

        [TestMethod]
        public void Given_a_scriptpubkey_When_parsed_Then_should_compare_to_address()
        {
            var jsonTransaction = File.OpenText(@"..\..\Resources\Transaction.json").ReadToEnd();
            var trx = JsonSerializer.DeSerialize<DecodedRawTransaction>(jsonTransaction);

            // Check we can extract the to address
            var pubkeyBytes = CryptoUtil.ConvertHex(trx.VOut.First().ScriptPubKey.Hex);
            var pubkey = new Script(pubkeyBytes);
            Assert.AreEqual("DUP HASH160 PUSHDATA(20)[ec4cca42352bc39020ba2da4b49bff6a72b6a079] EQUALVERIFY CHECKSIG", pubkey.ToString());
            var toAddr = Address.Create(parameters, pubkey.GetPubKeyHash());
            Assert.AreEqual("1NYSTjub949EBYTc6oDgzTorkDp4ZMbyB7", toAddr.ToString());
        }

        [TestMethod]
        public void Given_a_publickey_When_parsed_Then_should_match_script_address()
        {
            // pay to pubkey
            var toKey = new BitcoinPrivateKey(parameters, "KzXzfn1AKhA9XaBtr2enevcsw1w3NXWXUDgKKpqa8QyVi1cKYKdS").Key;
            var toAddress = toKey.ToPublicKey().ToAddress(parameters);
            Assert.AreEqual(toAddress.ToString(), ScriptBuilder.CreateOutputScript(toKey).GetToAddress(parameters, true).ToString());

            // pay to pubkey hash
            Assert.AreEqual(toAddress.ToString(), ScriptBuilder.CreateOutputScript(toAddress).GetToAddress(parameters, true).ToString());

            // pay to script hash
            ////Script p2ShScript = ScriptBuilder.CreateP2SHOutputScript(new byte[20]);
            ////Address scriptAddress = Address.Create(parameters, p2ShScript.GetPubKeyHash());
            ////Assert.AreEqual(scriptAddress, p2ShScript.GetToAddress(parameters, true));
        }

        [TestMethod]
        [ExpectedException(typeof(ScriptException))]
        public void Given_a_invalid_publickey_When_parsed_Then_should_fail()
        {
            var key = new BitcoinPrivateKey(parameters, "KzXzfn1AKhA9XaBtr2enevcsw1w3NXWXUDgKKpqa8QyVi1cKYKdS").Key;
            ScriptBuilder.CreateOutputScript(key).GetToAddress(parameters, false);
        }

        [TestMethod]
        public void given_a_dummy_transaction_When_createing_input_scripts_Then_all_should_pass()
        {
            var dummySig = TransactionSignature.Dummy();
            var key = new BitcoinPrivateKey(parameters, "KzXzfn1AKhA9XaBtr2enevcsw1w3NXWXUDgKKpqa8QyVi1cKYKdS").Key;

            // pay-to-pubkey
            var inputScript = ScriptBuilder.CreateInputScript(dummySig);
            Assert.IsTrue(inputScript.Chunks.ElementAt(0).Data.SequenceEqual(dummySig.EncodeToBitcoin()));
            inputScript = ScriptBuilder.CreateInputScript(null);
            Assert.AreEqual(inputScript.Chunks.ElementAt(0).Opcode, ScriptOpCodes.OP_0);

            // pay-to-address
            inputScript = ScriptBuilder.CreateInputScript(dummySig, key);
            Assert.IsTrue(inputScript.Chunks.ElementAt(0).Data.SequenceEqual(dummySig.EncodeToBitcoin()));
            inputScript = ScriptBuilder.CreateInputScript(null, key);
            Assert.AreEqual(inputScript.Chunks.ElementAt(0).Opcode, ScriptOpCodes.OP_0);
            Assert.IsTrue(inputScript.Chunks.ElementAt(1).Data.SequenceEqual(key.PublicKeyBytes));
        }

        [TestMethod]
        public void NumberBuilderZero()
        {
            var builder = new ScriptBuilder();

            // 0 should encode directly to 0
            builder.Number(0);
            Assert.IsTrue(new byte[] { 0x00 // Pushed data
                                       }.SequenceEqual( builder.Build().GetProgram()));
        }

        [TestMethod]
        public void NumberBuilderPositiveOpCode()
        {
            var builder = new ScriptBuilder();

            builder.Number(5);
            Assert.IsTrue(new byte[] { 0x55 // Pushed data
                                       }.SequenceEqual(builder.Build().GetProgram()));
        }

        [TestMethod]
        public void NumberBuilderBigNum()
        {
            var builder = new ScriptBuilder();

            // 21066 should take up three bytes including the length byte
            // at the start
            builder.Number(0x524a);
            var num1 = new byte[] { 0x02, // Length of the pushed data
                                    0x4a, 0x52 // Pushed data
                                  };
            var num2 = builder.Build().GetProgram();
            Assert.IsTrue(num1.SequenceEqual(num2));

            // Test the trimming code ignores zeroes in the middle
            builder = new ScriptBuilder();
            builder.Number(0x110011);
            Assert.AreEqual(4, builder.Build().GetProgram().Length);

            // Check encoding of a value where signed/unsigned encoding differs
            // because the most significant byte is 0x80, and therefore a
            // sign byte has to be added to the end for the signed encoding.
            builder = new ScriptBuilder();
            builder.Number(0x8000);
            Assert.IsTrue(new byte[] { 0x03, // Length of the pushed data
                                       0x00, (byte)0x80, 0x00 // Pushed data
                                     }.SequenceEqual(builder.Build().GetProgram()));
        }

        [TestMethod]
        public void NumberBuilderNegative()
        {
            // Check encoding of a negative value
            var builder = new ScriptBuilder();
            builder.Number(-5);
            Assert.IsTrue(new byte[] { 0x01, // Length of the pushed data
                                         (byte)133 // Pushed data
                                       }.SequenceEqual( builder.Build().GetProgram()));
        }
    }
}
