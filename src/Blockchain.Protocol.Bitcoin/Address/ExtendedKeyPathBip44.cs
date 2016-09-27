// <copyright file="ExtendedKeyPathBip44.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Address
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Extension;

    #endregion

    /// <summary>
    /// A BIP44 helper for deriving keys.
    /// </summary>
    public class ExtendedKeyPathBip44 : ExtendedKeyPath
    {
        #region Public Properties
        public uint CoinIndex
        {
            get
            {
                return ExtendedKey.FromHadrendIndex(this.Index(1));
            }
        }

        public uint AddressIndex
        {
            get
            {
                return this.Index(4);
            }
        }

        public uint AccountIndex
        {
            get
            {
                return ExtendedKey.FromHadrendIndex(this.Index(2));
            }
        }

        public uint ChangeIndex
        {
            get
            {
                return this.Index(3);
            }
        }

        public ExtendedKeyPathBip44 Clone()
        {
            return new ExtendedKeyPathBip44 { Items = this.Items.ToList(), Path = string.Copy(this.Path) };
        }

        public ExtendedKeyPathBip44 AddChange(uint index)
        {
            Thrower.If(this.Items.Count() > 3).Throw<AddressException>("Change was already added");
            this.AddChild(index);
            return this;
        }

        public static ExtendedKeyPathBip44 ParseBip44(string path)
        {
            Guard.Require(path.StartsWith("m/"));

            var keyPath = new ExtendedKeyPathBip44 
            { 
                Path = path, 
                Items = path.Substring(2).Split('/').Select(ConvertPathItem).ToList() 
            };

            Guard.Require(keyPath.Index(0) == ExtendedKey.ToHadrendIndex(44));
            Guard.Require(keyPath.IsHardendIndex(1));
            Guard.Require(keyPath.IsHardendIndex(2));

            return keyPath;
        }

        public static ExtendedKeyPathBip44 CreateBip44(uint coinIndex, uint accountIndex = 0)
        {
            var keyPath = new ExtendedKeyPathBip44 { Items = new List<uint>() };

            //// populate the items according to the BIP44 path 
            //// [m/purpose'/coin_type'/account'/change/address_index]

            keyPath.Items.Add(ExtendedKey.ToHadrendIndex(44));
            keyPath.Items.Add(ExtendedKey.ToHadrendIndex(coinIndex));
            keyPath.Items.Add(ExtendedKey.ToHadrendIndex(accountIndex));

            return keyPath;
        }

        #endregion
    }
}