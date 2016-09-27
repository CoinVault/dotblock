// <copyright file="TestBip44.cs" company="SoftChains">
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
    #region Using Directives

    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Mnemonic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    /// <summary>
    /// Test class
    /// </summary>
    [TestClass]
    public class TestBip44
    {
        /// <summary>
        /// The test.
        /// </summary>
        [TestMethod]
        public void TestPathBip44()
        {
            const string ExpectedPubKey = "xpub6EncrYPyfQEpEmcMsHEnAXFQY5JkfMcyEWjExM8ppTtJ2y2TR7FfTjEVYxru1Ry9cYdsSkiPjZhv94KyJE8JT8bQuFKHpGzNV4cMscWeRLT";
            const string ExpectedPrvKey = "xprvA1oGT2s5q2gX2HXtmFhmoPJfz3UGFtu7sHoe9xjDG8MKAAhJsZwQuvv1hgSHsCV1CDzL6pq9cFPoWgh7pV4TNKwdzCKBvm6MX5YqaoPZWDu";
            const string Mnemonic = "thrive empower soon push mountain jeans chimney top jelly sorry beef hard napkin mule matrix today draft high vacuum exercise blind kitchen inflict abstract";

            ////var key = CryptoService.CreateNew32ByteKeyPushEntropy();
            ////var bip39 = new Bip39(Convert.FromBase64String(key.ConvertToString()));
            
            var bip39 = new Bip39(Mnemonic);
            var usersMasterSeed = bip39.SeedBytes;

            var prvkey = ExtendedKey.Create(ExtendedKey.Bitcoinseed, usersMasterSeed);
            var keyPath = ExtendedKeyPathBip44.CreateBip44(0).AddChild(0);

            var derrived = keyPath.Items.Aggregate(prvkey, (current, item) => current.GetChild(item));

            var prv = derrived.Serialize();
            var pub = derrived.GetPublicKey().Serialize();

            Assert.AreEqual(ExpectedPubKey, pub);
            Assert.AreEqual(ExpectedPrvKey, prv);
        }
    }
}
