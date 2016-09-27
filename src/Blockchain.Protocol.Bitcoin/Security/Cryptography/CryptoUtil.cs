// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoUtil.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Security.Cryptography
{
    #region Using Directives

    using System;
    using System.Security.Cryptography;
    using System.Text;

    #endregion

    /// <summary>
    /// Crypto utility to help with cryptography operations.
    /// </summary>
    public class CryptoUtil
    {
        /// <summary>
        /// The generate SHA a 512 signature.
        /// </summary>
        /// <param name="payLoad">
        /// The payLoad.
        /// </param>
        /// <param name="privateKey">
        /// The private key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GenerateSha512Signature(string payLoad, byte[] privateKey)
        {
            var hash = ComputeHmac512(privateKey, payLoad, Encoding.ASCII);
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        /// The get hash.
        /// </summary>
        /// <param name="privateKey">
        /// The key byte.
        /// </param>
        /// <param name="payLoad">
        /// The payLoad.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The hash.
        /// </returns>
        public static byte[] ComputeHmac512(byte[] privateKey, string payLoad, Encoding encoding)
        {
            return ComputeHmac512(privateKey, encoding.GetBytes(payLoad));
        }

        /// <summary>
        /// The get hash.
        /// </summary>
        /// <param name="privateKey">
        /// The key byte.
        /// </param>
        /// <param name="messageBytes">
        /// The payLoad.
        /// </param>
        /// <returns>
        /// The hash.
        /// </returns>
        public static byte[] ComputeHmac512(byte[] privateKey, byte[] messageBytes)
        {
            using (var hmacsha512 = new HMACSHA512(privateKey))
            {
                var result = hmacsha512.ComputeHash(messageBytes);

                return result;
            }
        }

        /// <summary>
        /// Compute the SHA256 hash using a secret key.
        /// </summary>
        /// <param name="key">
        /// The secret key.
        /// </param>
        /// <param name="value">
        /// The string to compute a hash for.
        /// </param>
        /// <param name="encoding">
        /// The encoding to be used to read the string.
        /// </param>
        /// <returns>
        /// The hashed message.
        /// </returns>
        public static byte[] ComputeHmac256(byte[] key, string value, Encoding encoding)
        {
            var bytes = encoding.GetBytes(value);
            using (var hashAlgorithm = new HMACSHA256(key))
            {
                return hashAlgorithm.ComputeHash(bytes);
            }
        }

        /// <summary>
        /// The SHA 256 Hash.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The hash.
        /// </returns>
        public static byte[] Sha256Hash(string value, Encoding encoding)
        {
            using (var hash = SHA256.Create())
            {
                var result = hash.ComputeHash(encoding.GetBytes(value));

                return result;
            }
        }

        /// <summary>
        /// The SHA 256 Hash.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The hash.
        /// </returns>
        public static byte[] Sha256Hash(byte[] value)
        {
            using (var hash = SHA256.Create())
            {
                var result = hash.ComputeHash(value);

                return result;
            }
        }

        /// <summary>
        /// The SHA 256 Hash.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The hash.
        /// </returns>
        public static byte[] Sha256HashTwice(byte[] value)
        {
            using (var hash = SHA256.Create())
            {
                var first = hash.ComputeHash(value);
                var result = hash.ComputeHash(first);

                return result;
            }
        }

        /// <summary>
        /// The zero hash.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The bytes.
        /// </returns>
        public static byte[] ZeroHash(int length)
        {
            return new byte[length];
        }

        /// <summary>
        /// The convert hex.
        /// </summary>
        /// <param name="hex">
        /// The hex.
        /// </param>
        /// <returns>
        /// The hex array.
        /// </returns>
        public static byte[] ConvertHex(string hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            
            for (var i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        /// <summary>
        /// The to hex.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
        }
    }
}
