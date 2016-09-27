// <copyright file="TestHierarchicalWallet.cs" company="SoftChains">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// </summary>
    [TestClass]
    public class TestHierarchicalWallet
    {
        /// <summary>
        /// </summary>
        [TestMethod]
        public void Bip32Test1()
        {
            RunTest(test1);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void Bip32Test2()
        {
            RunTest(test2);
        }

        /// <summary>
        /// </summary>
        /// <param name="test">
        /// </param>
        private static void RunTest(TestVector test)
        {
            var constHash = "Bitcoin seed".Select(Convert.ToByte).ToArray();

            var headers = new CoinParameters { PublicKeyAddressVersion = 0, PrivateKeyVersion = 128 };

            var seed = CryptoUtil.ConvertHex(test.strHexMaster);
            var key = ExtendedKey.Create(constHash, seed);
            var pubkey = key.GetPublicKey();

            foreach (var derive in test.vDerive)
            {
                var skey = key.Serialize();
                var spubkey = pubkey.Serialize();

                Guard.Require(skey == derive.prv);
                Guard.Require(spubkey == derive.pub);

                var pkey = ExtendedKey.Parse(derive.prv);
                var ppubkey = ExtendedKey.Parse(derive.pub);

                var wif1 = pkey.GetKey(1).PrivateKey.ToWifKey(headers);
                var wif2 = key.GetKey(1).PrivateKey.ToWifKey(headers);
                Guard.Require(wif1 == wif2);

                var addr1 = ppubkey.GetKey(1).PublicKey.ToAddress(headers);
                var addr2 = pubkey.GetKey(1).ToAddress(headers);
                Guard.Require(addr1.ToString() == addr2.ToString());

                key = key.GetChild(derive.nChild);
                if ((derive.nChild & 0x80000000) == 0)
                {
                    var pubkeyn = pubkey.GetChild(derive.nChild);
                    pubkey = key.GetPublicKey();
                    Guard.Require(pubkey.Master.ToAddress(headers).ToString() == pubkeyn.Master.ToAddress(headers).ToString());
                }
                else
                {
                    pubkey = key.GetPublicKey();
                }
            }
        }

        /// <summary>
        /// </summary>
        class TestDerivation
        {
            /// <summary>
            /// </summary>
            public string pub;

            /// <summary>
            /// </summary>
            public string prv;

            /// <summary>
            /// </summary>
            public uint nChild;
        };

        /// <summary>
        /// </summary>
        class TestVector
        {
            /// <summary>
            /// </summary>
            public string strHexMaster;

            /// <summary>
            /// </summary>
            public List<TestDerivation> vDerive = new List<TestDerivation>();

            /// <summary>
            /// </summary>
            /// <param name="strHexMasterIn">
            /// </param>
            public TestVector(string strHexMasterIn)
            {
                this.strHexMaster = strHexMasterIn;
            }

            /// <summary>
            /// </summary>
            /// <param name="pub">
            /// </param>
            /// <param name="prv">
            /// </param>
            /// <param name="nChild">
            /// </param>
            /// <returns>
            /// </returns>
            public TestVector Add(string pub, string prv, uint nChild)
            {
                this.vDerive.Add(new TestDerivation());
                var der = this.vDerive.Last();
                der.pub = pub;
                der.prv = prv;
                der.nChild = nChild;
                return this;
            }
        };

        /// <summary>
        /// </summary>
        static TestVector test1 =
          new TestVector("000102030405060708090a0b0c0d0e0f")
            .Add("xpub661MyMwAqRbcFtXgS5sYJABqqG9YLmC4Q1Rdap9gSE8NqtwybGhePY2gZ29ESFjqJoCu1Rupje8YtGqsefD265TMg7usUDFdp6W1EGMcet8", 
             "xprv9s21ZrQH143K3QTDL4LXw2F7HEK3wJUD2nW2nRk4stbPy6cq3jPPqjiChkVvvNKmPGJxWUtg6LnF5kejMRNNU3TGtRBeJgk33yuGBxrMPHi", 
             0x80000000)
            .Add("xpub68Gmy5EdvgibQVfPdqkBBCHxA5htiqg55crXYuXoQRKfDBFA1WEjWgP6LHhwBZeNK1VTsfTFUHCdrfp1bgwQ9xv5ski8PX9rL2dZXvgGDnw", 
             "xprv9uHRZZhk6KAJC1avXpDAp4MDc3sQKNxDiPvvkX8Br5ngLNv1TxvUxt4cV1rGL5hj6KCesnDYUhd7oWgT11eZG7XnxHrnYeSvkzY7d2bhkJ7", 
             1)
            .Add("xpub6ASuArnXKPbfEwhqN6e3mwBcDTgzisQN1wXN9BJcM47sSikHjJf3UFHKkNAWbWMiGj7Wf5uMash7SyYq527Hqck2AxYysAA7xmALppuCkwQ", 
             "xprv9wTYmMFdV23N2TdNG573QoEsfRrWKQgWeibmLntzniatZvR9BmLnvSxqu53Kw1UmYPxLgboyZQaXwTCg8MSY3H2EU4pWcQDnRnrVA1xe8fs", 
             0x80000002)
            .Add("xpub6D4BDPcP2GT577Vvch3R8wDkScZWzQzMMUm3PWbmWvVJrZwQY4VUNgqFJPMM3No2dFDFGTsxxpG5uJh7n7epu4trkrX7x7DogT5Uv6fcLW5", 
             "xprv9z4pot5VBttmtdRTWfWQmoH1taj2axGVzFqSb8C9xaxKymcFzXBDptWmT7FwuEzG3ryjH4ktypQSAewRiNMjANTtpgP4mLTj34bhnZX7UiM", 
             2)
            .Add("xpub6FHa3pjLCk84BayeJxFW2SP4XRrFd1JYnxeLeU8EqN3vDfZmbqBqaGJAyiLjTAwm6ZLRQUMv1ZACTj37sR62cfN7fe5JnJ7dh8zL4fiyLHV", 
             "xprvA2JDeKCSNNZky6uBCviVfJSKyQ1mDYahRjijr5idH2WwLsEd4Hsb2Tyh8RfQMuPh7f7RtyzTtdrbdqqsunu5Mm3wDvUAKRHSC34sJ7in334", 
             1000000000)
            .Add("xpub6H1LXWLaKsWFhvm6RVpEL9P4KfRZSW7abD2ttkWP3SSQvnyA8FSVqNTEcYFgJS2UaFcxupHiYkro49S8yGasTvXEYBVPamhGW6cFJodrTHy", 
             "xprvA41z7zogVVwxVSgdKUHDy1SKmdb533PjDz7J6N6mV6uS3ze1ai8FHa8kmHScGpWmj4WggLyQjgPie1rFSruoUihUZREPSL39UNdE3BBDu76", 
             0);

        /// <summary>
        /// </summary>
        static TestVector test2 =
          new TestVector("fffcf9f6f3f0edeae7e4e1dedbd8d5d2cfccc9c6c3c0bdbab7b4b1aeaba8a5a29f9c999693908d8a8784817e7b7875726f6c696663605d5a5754514e4b484542")
            .Add("xpub661MyMwAqRbcFW31YEwpkMuc5THy2PSt5bDMsktWQcFF8syAmRUapSCGu8ED9W6oDMSgv6Zz8idoc4a6mr8BDzTJY47LJhkJ8UB7WEGuduB", 
             "xprv9s21ZrQH143K31xYSDQpPDxsXRTUcvj2iNHm5NUtrGiGG5e2DtALGdso3pGz6ssrdK4PFmM8NSpSBHNqPqm55Qn3LqFtT2emdEXVYsCzC2U", 
             0)
            .Add("xpub69H7F5d8KSRgmmdJg2KhpAK8SR3DjMwAdkxj3ZuxV27CprR9LgpeyGmXUbC6wb7ERfvrnKZjXoUmmDznezpbZb7ap6r1D3tgFxHmwMkQTPH", 
             "xprv9vHkqa6EV4sPZHYqZznhT2NPtPCjKuDKGY38FBWLvgaDx45zo9WQRUT3dKYnjwih2yJD9mkrocEZXo1ex8G81dwSM1fwqWpWkeS3v86pgKt", 
             0xFFFFFFFF)
            .Add("xpub6ASAVgeehLbnwdqV6UKMHVzgqAG8Gr6riv3Fxxpj8ksbH9ebxaEyBLZ85ySDhKiLDBrQSARLq1uNRts8RuJiHjaDMBU4Zn9h8LZNnBC5y4a", 
             "xprv9wSp6B7kry3Vj9m1zSnLvN3xH8RdsPP1Mh7fAaR7aRLcQMKTR2vidYEeEg2mUCTAwCd6vnxVrcjfy2kRgVsFawNzmjuHc2YmYRmagcEPdU9", 
             1)
            .Add("xpub6DF8uhdarytz3FWdA8TvFSvvAh8dP3283MY7p2V4SeE2wyWmG5mg5EwVvmdMVCQcoNJxGoWaU9DCWh89LojfZ537wTfunKau47EL2dhHKon", 
             "xprv9zFnWC6h2cLgpmSA46vutJzBcfJ8yaJGg8cX1e5StJh45BBciYTRXSd25UEPVuesF9yog62tGAQtHjXajPPdbRCHuWS6T8XA2ECKADdw4Ef", 
             0xFFFFFFFE)
            .Add("xpub6ERApfZwUNrhLCkDtcHTcxd75RbzS1ed54G1LkBUHQVHQKqhMkhgbmJbZRkrgZw4koxb5JaHWkY4ALHY2grBGRjaDMzQLcgJvLJuZZvRcEL", 
             "xprvA1RpRA33e1JQ7ifknakTFpgNXPmW2YvmhqLQYMmrj4xJXXWYpDPS3xz7iAxn8L39njGVyuoseXzU6rcxFLJ8HFsTjSyQbLYnMpCqE2VbFWc", 
             2)
            .Add("xpub6FnCn6nSzZAw5Tw7cgR9bi15UV96gLZhjDstkXXxvCLsUXBGXPdSnLFbdpq8p9HmGsApME5hQTZ3emM2rnY5agb9rXpVGyy3bdW6EEgAtqt", 
             "xprvA2nrNbFZABcdryreWet9Ea4LvTJcGsqrMzxHx98MMrotbir7yrKCEXw7nadnHM8Dq38EGfSh6dqA9QWTyefMLEcBYJUuekgW4BYPJcr9E7j", 
             0);
    }
}
