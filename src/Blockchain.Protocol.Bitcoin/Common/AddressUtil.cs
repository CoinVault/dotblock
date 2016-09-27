// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddressUtil.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Common
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Extension;

    using Org.BouncyCastle.Crypto.Digests;

    #endregion

    /// <summary>
    /// The address utility.
    /// </summary>
    public class AddressUtil
    {
        #region Public Methods and Operators

        /// <summary>
        /// Calculates RIPEMD160(SHA256(input)). This is used in Address calculations.
        /// </summary>
        public static byte[] Sha256Hash160(byte[] input)
        {
            using (var hasher = new SHA256Managed())
            {
                var sha256 = hasher.ComputeHash(input);
                return ComputeDigest(new RipeMD160Digest(), sha256);
            }
        }

        private static byte[] ComputeDigest(GeneralDigest digest, byte[] input)
        {
            digest.BlockUpdate(input, 0, input.Length);
            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            return output;
        }

        /// <summary>
        /// The resolve.
        /// </summary>
        public static CoinParameters TryCreateCoinParameters(string coinTag)
        {
            var coinParameters = new CoinParameters { CoinTag = coinTag };
            if (AddressUtil.PopulateCoinParameters(coinParameters))
            {
                return coinParameters;
            }
            return null;
        }

        private static readonly Dictionary<string,List<int>> Params = new Dictionary<string, List<int>>()
        {
            // CoinTag -              pub, script, HD, prv
            { "BTC",   new List<int> {  0,   5,   0,  128 }},
            { "LTC",   new List<int> { 48,   5,   2,  176 }},
            { "NMC",   new List<int> { 52,  13,   7,  128 }},
            { "DOGE",  new List<int> { 30,  22,   3,  158 }},
            { "DASH",  new List<int> { 76,  16,   5,  204 }},
            { "PPC",   new List<int> { 55,   5,   6,  183 }},
            { "QRK",   new List<int> { 58,   9,  82,  186 }},
            { "RDD",   new List<int> { 61,   5,   4,  189 }},
            { "TRC",   new List<int> {  0,   5,  83,  128 }},
            { "XPM",   new List<int> { 23,  83,  24,  151 }},
            { "GRC",   new List<int> { 62,  85,  84,  190 }},
            { "MTR",   new List<int> { 50,  28,  93,  178 }},
            { "GB",    new List<int> { 38,  85,  94,  155 }},
            { "SHM",   new List<int> { 63,  85,  95,  191 }},
            { "CRX",   new List<int> { 28,  85,  96,  156 }},
            { "UBIQ",  new List<int> { 68,  72,  97,  142 }},
            { "ARG",   new List<int> { 23,   5,  45,  151 }},
            { "ZYD",   new List<int> { 81,   5, 103,  209 }},
            { "DLC",   new List<int> { 30,   5, 102,  158 }},
            { "STRAT", new List<int> { 63, 125, 105,  191 }},
            { "SH",    new List<int> { 63,   5, 106,  191 }}

            //{ "XXX",  new List<int> { -1,  -1,  -1,  -1 }},

        };

        /// <summary>
        /// The resolve.
        /// </summary>
        public static bool PopulateCoinParameters(CoinParameters param)
        {
            // places to look for the network parameters
            // https://github.com/bitcoin/bitcoin/blob/d612837814020ae832499d18e6ee5eb919a87907/src/chainparams.cpp
            // https://github.com/CrypticApplications/MTR-Update/blob/master/src/base58.h
            // example MTR => private key prefix is [128 + CBitcoinAddress::PUBKEY_ADDRESS]

            List<int> vals;
            if (Params.TryGetValue(param.CoinTag, out vals))
            {
                param.PublicKeyAddressVersion = vals.ElementAt(0);
                param.ScriptAddressVersion = vals.ElementAt(1);
                param.ExtendedKeyIndex = vals.ElementAt(2);
                param.PrivateKeyVersion = vals.ElementAt(3);

                Thrower.If(param.PrivateKeyVersion == -1).Throw<AddressException>();
                Thrower.If(param.ExtendedKeyIndex == -1).Throw<AddressException>();
                Thrower.If(param.PublicKeyAddressVersion == -1).Throw<AddressException>();

                return true;
            }

            return false;
        }

        #endregion
    }
}