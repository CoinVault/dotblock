// <copyright file="BitHelper.cs" company="SoftChains">
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
    #region Using Directives

    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;

    #endregion

    /// <summary>
    /// The bite helper that takes in consideration Endian.
    /// </summary>
    public static class BitHelper
    {
        public static byte[] GetBytes(uint val, bool littleEndian = true)
        {
            var res = BitConverter.GetBytes(val);

            if (BitConverter.IsLittleEndian == littleEndian)
            {
                return res;
            }
            
            return res.Reverse().ToArray();
        }

        public static byte[] GetBytes(long val, bool littleEndian = true)
        {
            var res = BitConverter.GetBytes(val);

            if (BitConverter.IsLittleEndian == littleEndian)
            {
                return res;
            }

            return res.Reverse().ToArray();
        }

        public static uint ToUInt32(byte[] bytes, bool isLittleEndian = true)
        {
            if (BitConverter.IsLittleEndian == isLittleEndian)
            {
                return BitConverter.ToUInt32(bytes, 0);
            }

            return BitConverter.ToUInt32(bytes.Reverse().ToArray(), 0);
        }

        public static int ToInt32(byte[] bytes, bool isLittleEndian = true)
        {
            if (BitConverter.IsLittleEndian == isLittleEndian)
            {
                return BitConverter.ToInt32(bytes, 0);
            }

            return BitConverter.ToInt32(bytes.Reverse().ToArray(), 0);
        }

        public static byte[] SwapEndianBytes(byte[] bytes)
        {
            byte[] output = new byte[bytes.Length];

            int index = 0;

            foreach (byte b in bytes)
            {
                byte[] ba = { b };
                BitArray bits = new BitArray(ba);

                int newByte = 0;
                if (bits.Get(7)) newByte++;
                if (bits.Get(6)) newByte += 2;
                if (bits.Get(5)) newByte += 4;
                if (bits.Get(4)) newByte += 8;
                if (bits.Get(3)) newByte += 16;
                if (bits.Get(2)) newByte += 32;
                if (bits.Get(1)) newByte += 64;
                if (bits.Get(0)) newByte += 128;

                output[index] = Convert.ToByte(newByte);

                index++;
            }

            return output;
        }

        public static long Available(this BinaryReader reader)
        {
            return reader.BaseStream.Length - reader.BaseStream.Position;
        }
    }
}
