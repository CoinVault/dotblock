// <copyright file="Base58Encoding.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Common
{
    using System;
    using System.Linq;
    using System.Numerics;
    using System.Security.Cryptography;

    using Blockchain.Protocol.Bitcoin.Extension;

    // Implements https://en.bitcoin.it/wiki/Base58Check_encoding
    public static class Base58Encoding
    {
        public const int CheckSumSizeInBytes = 4;

        public static byte[] AddCheckSum(byte[] data)
        {
            Guard.Require(data != null);

            byte[] checkSum = GetCheckSum(data);
            byte[] dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);
            return dataWithCheckSum;
        }

        //Returns null if the checksum is invalid
        public static byte[] VerifyAndRemoveCheckSum(byte[] data)
        {
            Guard.Require(data != null);

            byte[] result = ArrayHelpers.SubArray(data, 0, data.Length - CheckSumSizeInBytes);
            byte[] givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CheckSumSizeInBytes);
            byte[] correctCheckSum = GetCheckSum(result);
            if (givenCheckSum.SequenceEqual(correctCheckSum))
                return result;
            else
                return null;
        }

        private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string Encode(byte[] data)
        {
            Guard.Require(data != null);

            // Decode byte[] to BigInteger
            BigInteger intData = 0;
            for (int i = 0; i < data.Length; i++)
            {
                intData = intData * 256 + data[i];
            }

            // Encode BigInteger to Base58 string
            string result = "";
            while (intData > 0)
            {
                int remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (int i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }
            return result;
        }

        public static string EncodeWithCheckSum(byte[] data)
        {
            Guard.Require(data != null);
            return Encode(AddCheckSum(data));
        }

        public static byte[] Decode(string s)
        {
            Guard.Require(s != null);

            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (int i = 0; i < s.Length; i++)
            {
                int digit = Digits.IndexOf(s[i]); //Slow
                if (digit < 0)
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", s[i], i));
                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            int leadingZeroCount = s.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
                intData.ToByteArray()
                .Reverse()// to big endian
                .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
            return result;
        }

        // Throws `FormatException` if s is not a valid Base58 string, or the checksum is invalid
        public static byte[] DecodeWithCheckSum(string s)
        {
            Guard.Require(s != null);
            
            var dataWithCheckSum = Decode(s);
            var dataWithoutCheckSum = VerifyAndRemoveCheckSum(dataWithCheckSum);
            
            if (dataWithoutCheckSum == null)
            {
                throw new FormatException("Base58 checksum is invalid");
            }

            return dataWithoutCheckSum;
        }

        private static byte[] GetCheckSum(byte[] data)
        {
            Guard.Require(data != null);

            using (SHA256 sha256 = new SHA256Managed())
            {
                byte[] hash1 = sha256.ComputeHash(data);
                byte[] hash2 = sha256.ComputeHash(hash1);


                var result = new byte[CheckSumSizeInBytes];
                Buffer.BlockCopy(hash2, 0, result, 0, result.Length);

                return result;
            }
        }
    }
}