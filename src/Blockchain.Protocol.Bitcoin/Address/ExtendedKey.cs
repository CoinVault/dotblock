// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedKey.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Address
{
    #region Using Directives

    using System;
    using System.IO;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Common;
    using Blockchain.Protocol.Bitcoin.Extension;
    using Blockchain.Protocol.Bitcoin.Security.Cryptography;

    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Math.EC;

    #endregion

    /// <summary>
    /// See BIP32 https://github.com/bitcoin/bips/blob/master/bip-0032.mediawiki for more details about Deterministic keys.
    /// </summary>
    public class ExtendedKey
    {
        private const uint HardendIndex = 0x80000000u;

        private static readonly byte[] Xprv = { 0x04, 0x88, 0xAD, 0xE4 };

        private static readonly byte[] Xpub = { 0x04, 0x88, 0xB2, 0x1E };

        /// <summary>
        /// The bitcoin seed.
        /// </summary>
        public static byte[] Bitcoinseed = "Bitcoin seed".Select(Convert.ToByte).ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedKey"/> class.
        /// </summary>
        public ExtendedKey(BitcoinKey key, byte[] chainCode, byte depth, byte[] parentFingerPrint, uint index)
        {
            this.Master = key;
            this.ChainCode = chainCode;
            this.ParentFingerPrint = parentFingerPrint;
            this.Depth = depth;
            this.Index = index;
        }

        public BitcoinKey Master { get; set; }

        public byte[] ChainCode { get; set; }

        public byte Depth { get; set; }

        public byte[] ParentFingerPrint { get; set; }

        public uint Index { get; set; }

        /// <summary>
        /// Gets a value indicating whether this child key is hardened.
        /// </summary>
        public bool IsHardened
        {
            get
            {
                return (this.Index & HardendIndex) != 0;
            }
        }

        /// <summary>
        /// The standard of creating an extended key from a given seed.
        /// </summary>
        public static ExtendedKey Create(byte[] hashKey, byte[] seed)
        {
            var computedHash = CryptoUtil.ComputeHmac512(hashKey, seed);

            var key = new BitcoinPrivateKey(computedHash.Take(32).ToArray());
            var chain = computedHash.Skip(32).ToArray();

            return new ExtendedKey(key, chain, 0, BitHelper.GetBytes(0), 0);
        }

        /// <summary>
        /// Convert a none hardened index to a hardened index
        /// </summary>
        /// <param name="index">
        /// The index to convert.
        /// </param>
        /// <returns>
        /// The hardened index
        /// </returns>
        public static uint ToHadrendIndex(uint index)
        {
            return (HardendIndex + index);
        }

        /// <summary>
        /// Convert a none hardened index to a hardened index
        /// </summary>
        /// <param name="index">
        /// The index to convert.
        /// </param>
        /// <returns>
        /// The hardened index
        /// </returns>
        public static uint FromHadrendIndex(uint index)
        {
            return (index - HardendIndex);
        }

        /// <summary>
        /// Convert a none hardened index to a hardened index
        /// </summary>
        /// <param name="index">
        /// The index to convert.
        /// </param>
        /// <returns>
        /// The hardened index
        /// </returns>
        public static bool IsHardendIndex(uint index)
        {
            return (index & HardendIndex) != 0;
        }

        public byte[] GenerateFingerPrint()
        {
            return this.Master.PublicKey.Hash160.Take(4).ToArray();
        }

        public BitcoinKey GetKey(uint index)
        {
            return this.GenerateKey(index).Master;
        }

        public ExtendedKey GetChild(uint index)
        {
            return this.GenerateKey(index);
        }

        /// <summary>
        /// Return the public key of this extended key.
        /// </summary>
        public ExtendedKey GetPublicKey()
        {
            return new ExtendedKey(this.Master.PublicKey, this.ChainCode, this.Depth, this.ParentFingerPrint, this.Index);
        }

        /// <summary>
        /// Check if this extended key has the private key.
        /// </summary>
        public bool IsPrivate()
        {
            return this.Master.HasPrivateKey;
        }

        public string Serialize()
        {
            using (var stream = new MemoryStream())
            {
                using (var binWriter = new BinaryWriter(stream))
                {
                    binWriter.Write(this.Master.HasPrivateKey ? Xprv : Xpub);
                    binWriter.Write(this.Depth);
                    binWriter.Write(this.ParentFingerPrint);
                    binWriter.Write(BitHelper.GetBytes(this.Index, false));
                    binWriter.Write(this.ChainCode);

                    if (this.Master.HasPrivateKey)
                    {
                        binWriter.Write(byte.MinValue);
                        binWriter.Write(this.Master.PrivateKey.Key.GetPrivKeyBytes());
                    }
                    else
                    {
                        binWriter.Write(this.Master.PublicKey.Bytes);
                    }

                    return Base58Encoding.EncodeWithCheckSum(stream.ToArray());
                }
            }
        }

        public static ExtendedKey Parse(string serialized)
        {
            byte[] data = Base58Encoding.DecodeWithCheckSum(serialized);
            Thrower.Condition<AddressException>(data.Length != 78, "invalid extended key");

            using (var stream = new MemoryStream(data))
            {
                using (var binReader = new BinaryReader(stream))
                {
                    var ext = binReader.ReadBytes(4);
                    Thrower.Condition<AddressException>(!(ext.SequenceEqual(Xprv) || ext.SequenceEqual(Xpub)), "invalid magic number for an extended key");
                    
                    var isPrivate = ext.SequenceEqual(Xprv);
                    var depth = binReader.ReadByte();
                    var fingerprint = binReader.ReadBytes(4);
                    var index = BitHelper.ToUInt32(binReader.ReadBytes(4), false);
                    var chainCode = binReader.ReadBytes(32);
                    var rawKey = binReader.ReadBytes(33);

                    BitcoinKey key;
                    if (isPrivate)
                    {
                        key = new BitcoinPrivateKey(rawKey.Skip(1).ToArray());
                    }
                    else
                    {
                        key = new BitcoinPublicKey(rawKey);
                    }

                    return new ExtendedKey(key, chainCode, depth, fingerprint, index);
                }
            }
        }

        private ExtendedKey GenerateKey(uint index)
        {
            Thrower.Condition<AddressException>((index & HardendIndex) != 0 && !this.Master.HasPrivateKey, "A public key can't derivate an hardened child");          

            byte[] extended;
            byte[] pub = this.Master.PublicKey.Bytes;
            if ((index & HardendIndex) == 0)
            {
                var sequenceBytes = BitHelper.GetBytes(index, false);
                extended = pub.ToArray().Concat(sequenceBytes).ToArray();
            }
            else
            {
                var priv = this.Master.PrivateKey.Bytes;
                var sequenceBytes = BitHelper.GetBytes(index, false);
                extended = (new byte[] { 0 }).Concat(priv.ToArray()).Concat(sequenceBytes).ToArray();
            }

            var leftRight = CryptoUtil.ComputeHmac512(this.ChainCode, extended);
            var leftKey = leftRight.Take(32).ToArray();
            var rightKey = leftRight.Skip(32).ToArray();

            BigInteger bigIntegerLeft = new BigInteger(1, leftKey);

            Thrower.Condition<AddressException>(bigIntegerLeft.CompareTo(EcKey.EcParams.N) >= 0, "This is rather unlikely, but it did just happen");          

            if (this.Master.HasPrivateKey)
            {
                BigInteger key = bigIntegerLeft.Add(new BigInteger(1, this.Master.PrivateKey.Bytes)).Mod(EcKey.EcParams.N);
                Thrower.Condition<AddressException>(key.Equals(BigInteger.Zero), "This is rather unlikely, but it did just happen");

                ////  fix the private key in case it needs padding 
                var keyBytes = new EcKey(key).GetPrivKeyBytes();

                return new ExtendedKey(new BitcoinPrivateKey(keyBytes), rightKey, (byte)(this.Depth + 1), this.GenerateFingerPrint(), index);
            }
            else
            {
                var qdecoded = EcKey.EcParams.Curve.DecodePoint(this.Master.PublicKey.Bytes);
                var key = new ECPublicKeyParameters("EC", qdecoded, EcKey.EcParams);

                var qkey = EcKey.EcParams.G.Multiply(bigIntegerLeft).Add(key.Q);
                Thrower.Condition<AddressException>(qkey.IsInfinity, "This is rather unlikely, but it did just happen");

                var point = new FpPoint(EcKey.EcParams.Curve, qkey.Normalize().XCoord, qkey.Normalize().YCoord, true);

                return new ExtendedKey(new BitcoinPublicKey(point.GetEncoded()), rightKey, (byte)(this.Depth + 1), this.GenerateFingerPrint(), index);
            }
        }
    }
}
