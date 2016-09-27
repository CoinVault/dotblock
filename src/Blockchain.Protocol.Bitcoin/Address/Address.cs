// <copyright file="Address.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Address
{
    #region Using Directives

    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;

    #endregion

    /// <summary>
    /// Class that manages a bitcoin address format.
    /// Bitcoin address is derived from an elliptic curve public key plus a set of network parameters.
    /// A standard address is built by taking the RIPE-MD160 hash of the public key bytes, with a version prefix and a
    /// checksum suffix, then encoding it textually as base58. The version prefix is used to both denote the network for
    /// which the address is valid (see {@link NetworkParameters}, and also to indicate how the bytes inside the address
    /// should be interpreted.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// An address is a RIPEMD160 hash of a public key, therefore is always 160 bits or 20 bytes.
        /// </summary>
        private const int Length = 20;

        /// <summary>
        /// Define the coin this address related to.
        /// </summary>
        public CoinParameters CoinParameters { get; set; }

        /// <summary>
        /// The address version pub key or script (if null will default to pub key)
        /// </summary>
        public int? AddressVersion { get; set; }

        /// <summary>
        /// The public key hash of the address.
        /// </summary>
        public byte[] Hash160 { get; set; }

        /// <summary>
        /// Create a bitcoin address from a public key RIPEMD160 hash
        /// </summary>
        public static Address Create(CoinParameters coinParameters, byte[] hash160)
        {
            return new Address { Hash160 = hash160, CoinParameters = coinParameters };
        }

        /// <summary>
        /// Create a bitcoin address from a public key RIPEMD160 hash and specify if it is a Script or a Pub key
        /// </summary>
        public static Address Create(CoinParameters coinParameters, byte[] hash160, bool isScript)
        {
            return new Address
                       {
                           Hash160 = hash160, 
                           CoinParameters = coinParameters,
                           AddressVersion = isScript ? 
                            coinParameters.ScriptAddressVersion :
                            coinParameters.PublicKeyAddressVersion
                       };
        }

        /// <summary>
        /// Create a bitcoin address from an imported string.
        /// </summary>
        public static Address Create(CoinParameters coinParameters, string address)
        {
            var decoded = Base58Encoding.DecodeWithCheckSum(address);
            var version = decoded.First();
            var hash = decoded.Skip(1).ToArray();

            Thrower.If(hash.Length != Length).Throw<AddressException>("Invalid length? expected={0} found={1}", Length, hash.Length);
            Thrower.If(
                version != coinParameters.PublicKeyAddressVersion && 
                version != coinParameters.ScriptAddressVersion)
                .Throw<AddressException>("Mismatched version number, trying to cross networks? expected={0},{1} found={2}", coinParameters.PublicKeyAddressVersion, coinParameters.ScriptAddressVersion, version);

            return new Address { Hash160 = hash, CoinParameters = coinParameters, AddressVersion = version };
        }

        /// <summary>
        /// Convert the public key hash to a bitcoin encoded address.
        /// </summary>
        public override string ToString()
        {
            Guard.Require(this.CoinParameters.IsNotNull());
            Guard.Require(this.CoinParameters.PublicKeyAddressVersion < 256 && this.CoinParameters.PublicKeyAddressVersion >= 0, "PublicKeyAddressVersion");
            Guard.Require(this.CoinParameters.ScriptAddressVersion < 256 && this.CoinParameters.ScriptAddressVersion >= 0, "ScriptAddressVersion");

            var keywithVersion = new[] { (byte)(this.AddressVersion ?? this.CoinParameters.PublicKeyAddressVersion) }.Concat(this.Hash160).ToArray();

            return Base58Encoding.EncodeWithCheckSum(keywithVersion);
        }

        /// <summary>
        /// Check if this address is script
        /// </summary>
        public bool IsScriptAddress()
        {
            return this.AddressVersion.HasValue && 
                this.AddressVersion != this.CoinParameters.PublicKeyAddressVersion;
        }
    }
}
