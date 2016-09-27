// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Extension
{
    #region Using Directives

    using System;
    using System.Diagnostics;
    using System.Globalization;

    #endregion

    /// <summary>
    /// This class defines extension methods.
    /// </summary>
    [DebuggerStepThrough]
    public static class Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Implemented a convert on an object, this should be similar to the "IEnumerable.Select" functionality in the Linq namespace applied on a single object.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="func">
        /// The action.
        /// </param>
        /// <typeparam name="TIn">
        /// The in type.
        /// </typeparam>
        /// <typeparam name="TOut">
        /// The out type.
        /// </typeparam>
        /// <returns>
        /// The type returned.
        /// </returns>
        public static TOut Convert<TIn, TOut>(this TIn source, Func<TIn, TOut> func) where TIn : class
        {
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return func(source);
        }

        /// <summary>
        /// Check an object is not null.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// True if has a value.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsNotNull(this object value)
        {
            return value != null;
        }

        /// <summary>
        /// Check a string has a value using the 'IsNullOrEmpty' method.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// True if has a value.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsNotNullOrEmpty(this string value) 
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Check an object is not null.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// True if has a value.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsNull(this object value)
        {
            return value == null;
        }

        /// <summary>
        /// Check a string has a value using the 'IsNullOrEmpty' method.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// True if has a value.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Provides an extension method for formatting strings with arguments.
        /// </summary>
        /// <param name="format">
        /// The string to format.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        [DebuggerStepThrough]
        public static string StringFormat(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Provides an extension method for formatting strings with arguments.
        /// </summary>
        /// <param name="value">
        /// The string to format.
        /// </param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        [DebuggerStepThrough]
        public static string GuidFormat(this string value)
        {
            return new Guid(value).ToString("N");
        }

        /// <summary>
        /// To Enumeration.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// The Enumeration type.
        /// </typeparam>
        /// <returns>
        /// The enumeration object.
        /// </returns>
        [DebuggerStepThrough]
        public static T ToEnum<T>(this string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// To Enumeration.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// The Enumeration type.
        /// </typeparam>
        /// <returns>
        /// The enumeration object.
        /// </returns>
        [DebuggerStepThrough]
        public static T ToEnumOrDefault<T>(this string value) where T : struct
        {
            T ret;
            return Enum.TryParse(value, out ret) ? ret : default(T);
        }

        /// <summary>
        /// The unix time stamp to date time.
        /// </summary>
        /// <param name="unixTimeStamp">
        /// The unix time stamp.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            return UnixUtils.UnixTimestampToDate(unixTimeStamp);
        }

        /// <summary>
        /// The unix time stamp to date time.
        /// </summary>
        /// <param name="timeStamp">
        /// The date stamp.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static long UnixTimeStampFromDateTime(this DateTime timeStamp)
        {
            // Unix timestamp is seconds past epoch
            return UnixUtils.DateToUnixTimestamp(timeStamp);
        }

        #endregion
    }
}