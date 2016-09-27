// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Guard.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Extension
{
    #region Using Directives

    using System;
    using System.Diagnostics;

    #endregion

    /// <summary>
    /// Represents a paged result.
    /// </summary>
    [DebuggerStepThrough]
    public class Guard
    {
        #region Public Methods and Operators

        /// <summary>
        /// Argument Null Exception helper.
        /// </summary>
        /// <typeparam name="T">
        /// The type to check.
        /// </typeparam>
        /// <param name="obj">
        /// The condition.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void CheckType<T>(object obj, string message)
        {
            if (!(obj is T))
            {
                throw new InvalidCastException(string.Format("Can not cast '{0}' to '{1}' : {2}", obj.GetType(), typeof(T), message));
            }
        }

        /// <summary>
        /// Ensures an argument is valid.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Ensures an argument is valid.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public static void Require(bool condition)
        {
            if (!condition)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// The are equal.
        /// </summary>
        /// <param name="expected">
        /// The expected.
        /// </param>
        /// <param name="actual">
        /// The actual.
        /// </param>
        /// <typeparam name="T">
        /// The type to check.
        /// </typeparam>
        public static void AreEqual<T>(T expected, T actual)
        {
            Require(expected.Equals(actual));
        }

        #endregion
    }
}