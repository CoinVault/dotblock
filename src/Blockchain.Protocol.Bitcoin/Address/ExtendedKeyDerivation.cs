// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedKeyDerivation.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Address
{
    using System.Collections.Generic;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Extension;

    /// <summary>
    /// The ExtendedKey Derivation derive keys from a master key.
    /// </summary>
    public class ExtendedKeyDerivation 
    {
        public ExtendedKey ExtendedKey { get; set; }

        public ExtendedKeyPath PathItem { get; set; }

        public static IEnumerable<ExtendedKeyDerivation> Derive(ExtendedKey masterKey, IEnumerable<ExtendedKeyPath> pathItems)
        {
            Guard.Require(masterKey.Depth == 0);

            // calculate the child keys based on the path items
            return from keyPath in pathItems
                   let iterator = masterKey
                   let keyIetrator = keyPath.Items.Aggregate(iterator, (current, item) => current.GetChild(item))
                   select new ExtendedKeyDerivation { ExtendedKey = keyIetrator, PathItem = keyPath };
        }

        public static IEnumerable<BitcoinPublicKey> DeriveBip44Account(ExtendedKey extKey, IEnumerable<ExtendedKeyPathBip44> pathItems)
        {
            Guard.Require(extKey.Depth == 3);
            Guard.Require(extKey.IsHardened);

            //// the pub key we received should be [m/44'/coin-index'/account']
            //// to support multi account wallets we store the account level pub key

            foreach (var pathItem in pathItems)
            {
                Guard.Require(extKey.Index == ExtendedKey.ToHadrendIndex(pathItem.AccountIndex));

                //// use the chain index and address index to generate the key
                //// this will yield a public key [m/44'/coin-index'/account'/chain/index]

                var derrivedKey = extKey.GetChild(pathItem.ChangeIndex).GetKey(pathItem.AddressIndex).PublicKey;

                yield return derrivedKey;
            }
        }
    }
}