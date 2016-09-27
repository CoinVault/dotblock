// <copyright file="BitcoinPublicKey.cs" company="SoftChains">
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
    #region

    using System;

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;

    #endregion

    /// <summary>
    /// A Bitcoin public key is fundamentally derived from an elliptic curve public key and a set of network parameters.
    /// </summary>
    public class BitcoinPublicKey : BitcoinKey
    {
        /// <summary>
        /// An address is a RIPEMD160 hash of a public key, therefore is always 160 bits or 20 bytes.
        /// </summary>
        public const int Length = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitcoinPublicKey"/> class. 
        /// </summary>
        /// <param name="keyBytes">
        /// The pub key.
        /// </param>
        /// <param name="compressed">
        /// The compressed.
        /// </param>
        public BitcoinPublicKey(byte[] keyBytes, bool compressed = true)
            : base(keyBytes)
        {
            this.Compressed = compressed;
        }

        public override byte[] Hash160
        {
            get
            {
                var hash160 = AddressUtil.Sha256Hash160(this.Bytes);
                Thrower.Condition<ArgumentException>(hash160.Length != Length, "Addresses are 160-bit hashes, so you must provide 20 bytes");
                return hash160;
            }
        }

        public override bool HasPrivateKey
        {
            get
            {
                return false;
            }
        }
    }
}