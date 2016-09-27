// <copyright file="Range.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591
namespace Blockchain.Protocol.Bitcoin.Extension
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    /// The range.
    /// </summary>
    public static class Range
    {
        /// <summary>
        /// The s byte.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<sbyte> SByte(sbyte from, sbyte to, int step)
        {
            return Int32(@from, to, step).Select(i => (sbyte)i);
        }

        /// <summary>
        /// The byte.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<byte> Byte(byte from, byte to, int step)
        {
            return Int32(@from, to, step).Select(i => (byte)i);
        }

        /// <summary>
        /// The char.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<char> Char(char from, char to, int step)
        {
            return Int32(@from, to, step).Select(i => (char)i);
        }

        /// <summary>
        /// The int 16.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<short> Int16(short from, short to, int step)
        {
            return Int32(@from, to, step).Select(i => (short)i);
        }

        /// <summary>
        /// The u int 16.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<ushort> UInt16(ushort from, ushort to, int step)
        {
            return Int32(@from, to, step).Select(i => (ushort)i);
        }

        /// <summary>
        /// The int 32.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<int> Int32(int from, int to, int step)
        {
            if (step <= 0) step = (step == 0) ? 1 : -step;
            if (@from <= to)
            {
                for (int i = @from; i <= to; i += step) yield return i;
            }
            else
            {
                for (int i = @from; i >= to; i -= step) yield return i;
            }
        }

        /// <summary>
        /// The u int 32.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<uint> UInt32(uint from, uint to, uint step)
        {
            if (step == 0U) step = 1U;
            if (@from <= to)
            {
                for (uint ui = @from; ui <= to; ui += step) yield return ui;
            }
            else
            {
                for (uint ui = @from; ui >= to; ui -= step) yield return ui;
            }
        }

        /// <summary>
        /// The int 64.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<long> Int64(long from, long to, long step)
        {
            if (step <= 0L) step = (step == 0L) ? 1L : -step;

            if (@from <= to)
            {
                for (long l = @from; l <= to; l += step) yield return l;
            }
            else
            {
                for (long l = @from; l >= to; l -= step) yield return l;
            }
        }

        /// <summary>
        /// The u int 64.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<ulong> UInt64(ulong from, ulong to, ulong step)
        {
            if (step == 0UL) step = 1UL;

            if (@from <= to)
            {
                for (ulong ul = @from; ul <= to; ul += step) yield return ul;
            }
            else
            {
                for (ulong ul = @from; ul >= to; ul -= step) yield return ul;
            }
        }

        /// <summary>
        /// The single.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<float> Single(float from, float to, float step)
        {
            if (step <= 0.0f) step = (step == 0.0f) ? 1.0f : -step;

            if (@from <= to)
            {
                for (float f = @from; f <= to; f += step) yield return f;
            }
            else
            {
                for (float f = @from; f >= to; f -= step) yield return f;
            }
        }

        /// <summary>
        /// The double.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<double> Double(double from, double to, double step)
        {
            if (step <= 0.0) step = (step == 0.0) ? 1.0 : -step;

            if (@from <= to)
            {
                for (double d = @from; d <= to; d += step) yield return d;
            }
            else
            {
                for (double d = @from; d >= to; d -= step) yield return d;
            }
        }

        /// <summary>
        /// The decimal.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<decimal> Decimal(decimal from, decimal to, decimal step)
        {
            if (step <= 0.0m) step = (step == 0.0m) ? 1.0m : -step;

            if (@from <= to)
            {
                for (decimal m = @from; m <= to; m += step) yield return m;
            }
            else
            {
                for (decimal m = @from; m >= to; m -= step) yield return m;
            }
        }

        /// <summary>
        /// The date time.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<DateTime> DateTime(DateTime from, DateTime to, double step)
        {
            if (step <= 0.0) step = (step == 0.0) ? 1.0 : -step;

            if (@from <= to)
            {
                for (DateTime dt = @from; dt <= to; dt = dt.AddDays(step)) yield return dt;
            }
            else
            {
                for (DateTime dt = @from; dt >= to; dt = dt.AddDays(-step)) yield return dt;
            }
        }

        /// <summary>
        /// The s byte.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<sbyte> SByte(sbyte from, sbyte to)
        {
            return SByte(@from, to, 1);
        }

        /// <summary>
        /// The byte.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<byte> Byte(byte from, byte to)
        {
            return Byte(@from, to, 1);
        }

        /// <summary>
        /// The char.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<char> Char(char from, char to)
        {
            return Char(@from, to, 1);
        }

        /// <summary>
        /// The int 16.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<short> Int16(short from, short to)
        {
            return Int16(@from, to, 1);
        }

        /// <summary>
        /// The u int 16.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<ushort> UInt16(ushort from, ushort to)
        {
            return UInt16(@from, to, 1);
        }

        /// <summary>
        /// The int 32.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<int> Int32(int from, int to)
        {
            return Int32(@from, to, 1);
        }

        /// <summary>
        /// The u int 32.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<uint> UInt32(uint from, uint to)
        {
            return UInt32(@from, to, 1U);
        }

        /// <summary>
        /// The int 64.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<long> Int64(long from, long to)
        {
            return Int64(@from, to, 1L);
        }

        /// <summary>
        /// The u int 64.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<ulong> UInt64(ulong from, ulong to)
        {
            return UInt64(@from, to, 1UL);
        }

        /// <summary>
        /// The single.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<float> Single(float from, float to)
        {
            return Single(@from, to, 1.0f);
        }

        /// <summary>
        /// The double.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<double> Double(double from, double to)
        {
            return Double(@from, to, 1.0);
        }

        /// <summary>
        /// The decimal.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<decimal> Decimal(decimal from, decimal to)
        {
            return Decimal(@from, to, 1.0m);
        }

        /// <summary>
        /// The date time.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <returns>
        /// The IEnumerable.
        /// </returns>
        public static IEnumerable<DateTime> DateTime(DateTime from, DateTime to)
        {
            return DateTime(@from, to, 1.0);
        }
    }
}