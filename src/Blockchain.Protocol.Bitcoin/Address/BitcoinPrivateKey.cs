// <copyright file="BitcoinPrivateKey.cs" company="SoftChains">
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

    using System;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;

    using Org.BouncyCastle.Math;

    #endregion

    /// <summary>
    /// Parses and generates private keys in the form used by the BitCoin command. This is the private key
    /// bytes with a header byte and 4 checksum bytes at the end.
    /// </summary>
    public class BitcoinPrivateKey : BitcoinKey
    {
        private const int Length = 32;

        /// <summary>
        /// The elliptic curve key.
        /// </summary>
        private EcKey eckey;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitcoinPrivateKey"/> class. 
        /// Parses the given private key as created by the BitCoin C++ RPC.
        /// </summary>
        /// <param name="coinParameters">
        /// The expected header parameters of the key. If you don't care, provide null.
        /// </param>
        /// <param name="encoded">
        /// The base58 encoded string.
        /// </param>
        public BitcoinPrivateKey(CoinParameters coinParameters, string encoded)
        {
            var decoded = Base58Encoding.DecodeWithCheckSum(encoded);
            var version = decoded.First();

            Thrower.Condition<ArgumentException>(coinParameters.PrivateKeyVersion != version, string.Format("Mismatched version number, trying to cross networks? expected={0} found={1}", coinParameters.PrivateKeyVersion, version));

            var bytes = decoded.Skip(1).ToArray();
            this.Compressed = false;

            if (bytes.Length == 33)
            {
                // private key associated with a compressed public key
                Thrower.Condition<ArgumentException>(bytes.Last() != CompressedBytes.First(), string.Format("Invalid private key"));

                bytes = bytes.Take(32).ToArray();
                this.Compressed = true;
            }

            // 256 bit keys
            Thrower.Condition<ArgumentException>(bytes.Length != Length, string.Format("Keys are 256 bits, so you must provide {0} bytes, got {1}", Length, bytes.Length));

            this.Bytes = bytes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitcoinPrivateKey"/> class.
        /// </summary>
        /// <param name="keyBytes">
        /// The key bytes.
        /// </param>
        public BitcoinPrivateKey(byte[] keyBytes)
            : base(keyBytes)
        {
            // 256 bit keys
            Thrower.Condition<ArgumentException>(keyBytes.Length != Length, string.Format("Keys are 256 bits, so you must provide {0} bytes, got {1}", Length, keyBytes.Length));
        }

        /// <summary>
        /// Gets the ECKey created from this encoded private key.
        /// </summary>
        public EcKey Key
        {
            get
            {
                return this.eckey ?? (this.eckey = new EcKey(new BigInteger(1, this.Bytes), this.Compressed));
            }
        }

        /// <summary>
        /// Gets the hash 160.
        /// </summary>
        public override byte[] Hash160
        {
            get
            {
                return this.PublicKey.Hash160;
            }
        }

        /// <summary>
        /// Gets a value indicating whether has private key.
        /// </summary>
        public override bool HasPrivateKey
        {
            get
            {
                return true;
            }
        }
    }
}