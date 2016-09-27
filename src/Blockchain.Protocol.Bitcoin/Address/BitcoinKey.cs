// <copyright file="BitcoinKey.cs" company="SoftChains">
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

    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;

    #endregion

    /// <summary>
    /// In BitCoin the following format is often used to represent some type of key:<p/>
    /// <pre>[one version byte] [data bytes] [4 checksum bytes]</pre><p/>
    /// and the result is then Base58 encoded. This format is used for addresses, and private keys exported using the command tool.
    /// </summary>
    public abstract class BitcoinKey
    {
        /// <summary>
        /// The compressed bytes.
        /// </summary>
        protected static byte[] CompressedBytes = new[] { (byte)0x01 }.ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="BitcoinKey"/> class.
        /// </summary>
        protected BitcoinKey()
        {
            this.Compressed = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitcoinKey"/> class.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        protected BitcoinKey(byte[] bytes)
        {
            Guard.Require(bytes.Length >= 0, "bytes");
           
            this.Bytes = bytes;
            this.Compressed = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use a compressed key.
        /// This corresponds to the public key being compressed, 
        /// a compressed key has the private key encoded with an additional sign 0x01.
        /// </summary>
        public bool Compressed { get; set; }

        /// <summary>
        /// Gets a value indicating whether is this key has the private.
        /// </summary>
        public abstract bool HasPrivateKey { get; }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        public BitcoinPublicKey PublicKey
        {
            get
            {
                return this.HasPrivateKey ? new BitcoinPublicKey(this.PrivateKey.Key.PublicKeyBytes) : this as BitcoinPublicKey;
            }
        }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        public BitcoinPrivateKey PrivateKey
        {
            get
            {
                Guard.Require(this.HasPrivateKey);
                return this as BitcoinPrivateKey;
            }
        }

        /// <summary>
        /// Gets the (big endian) 20 byte hash that is the core of a BitCoin address.
        /// </summary>
        /// <remarks>
        /// It has several possible representations:<p/>
        /// <ol>
        ///   <li>The raw public key bytes themselves.</li>
        ///   <li>RIPEMD160 hash of the public key bytes.</li>
        ///   <li>A base58 encoded "human form" that includes a version and check code, to guard against typos.</li>
        /// </ol><p/>
        /// One may question whether the base58 form is really an improvement over the hash160 form, given
        /// they are both very unfriendly for typists. More useful representations might include QR codes
        /// Note that an address is specific to a network because the first byte is a discriminator value.
        /// </remarks>
        public abstract byte[] Hash160 { get; }

        /// <summary>
        /// Gets or sets the raw bytes of this key.
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <param name="coinParameters">
        /// The key Headers.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public Address ToAddress(CoinParameters coinParameters)
        {
            return new Address { CoinParameters = coinParameters, Hash160 = this.Hash160 };
        }

        /// <summary>
        /// Import the private key to a wallet import format.
        /// </summary>
        /// <param name="coinParameters">
        /// The coin network to import to.
        /// </param>
        public string ToWifKey(CoinParameters coinParameters)
        {
            Guard.Require(coinParameters.PrivateKeyVersion < 256 && coinParameters.PrivateKeyVersion >= 0, "PrivateKeyVersion");
            Guard.Require(this.HasPrivateKey, "HasPrivateKey");

            var key = this.Bytes;
            var version = new[] { (byte)coinParameters.PrivateKeyVersion }.ToArray();

            key = version.Concat(key).ToArray();

            if (this.PrivateKey.Key.Compressed)
            {
                // compressed public keys so we append the zero to the private key
                key = key.Concat(CompressedBytes).ToArray();    
            }

            return Base58Encoding.EncodeWithCheckSum(key);
        }

        public override int GetHashCode()
        {
            return this.Bytes != null ? this.Bytes.Aggregate(1, (current, element) => (31 * current) + element) : 0;
        }
      
        public override bool Equals(object o)
        {
            if (!o.Is<BitcoinKey>())
            {
                return false;
            }

            var vcb = (BitcoinKey)o;
            return vcb.Bytes.SequenceEqual(this.Bytes);
        }
    }
}