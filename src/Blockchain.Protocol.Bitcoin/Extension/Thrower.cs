// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Thrower.cs" company="Dark Caesium">
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
    /// The thrower.
    /// </summary>
    [DebuggerStepThrough]
    public static class Thrower
    {
        /// <summary>
        /// Throw an exception if a condition is met.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see cref="Thrower"/>.
        /// </returns>
        public static InnerThrower If(bool condition)
        {
            return condition ? new InnerThrower() : null;
        }

        /// <summary>
        /// Throw an exception if a condition is met.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type.
        /// </typeparam>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public static void Condition<T>(bool condition) where T : Exception
        {
            if (condition)
            {
                Raise<T>();
            }
        }

        /// <summary>
        /// Throw an exception if a condition is met.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type.
        /// </typeparam>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="message">
        /// The exception message.
        /// </param>
        public static void Condition<T>(bool condition, string message) where T : Exception
        {
            if (condition)
            {
                Raise<T>(message);
            }
        }

        /// <summary>
        /// Throw an exception if a condition is met.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type.
        /// </typeparam>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// <param name="ex">
        /// Inner exception.
        /// </param>
        public static void Condition<T>(bool condition, string message, Exception ex) where T : Exception
        {
            if (condition)
            {
                Raise<T>(message, ex);
            }
        }

        /// <summary>
        /// Throw an exception.
        /// </summary>
        /// <typeparam name="T">The exception type.</typeparam>
        public static void Raise<T>() where T : Exception
        {
            var exception = (Exception)Activator.CreateInstance<T>();

            throw exception;
        }

        /// <summary>
        /// Throw an exception.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type.
        /// </typeparam>
        /// <param name="message">
        /// The exception message.
        /// </param>
        public static void Raise<T>(string message) where T : Exception
        {
            var exception = (Exception)Activator.CreateInstance(typeof(T), message);

            throw exception;
        }

        /// <summary>
        /// Throw an exception.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type.
        /// </typeparam>
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// <param name="ex">
        /// Inner exception.
        /// </param>
        public static void Raise<T>(string message, Exception ex) where T : Exception
        {
            var exception = (Exception)Activator.CreateInstance(typeof(T), message, ex);

            throw exception;
        }

        /// <summary>
        /// Throw an exception.
        /// </summary>
        /// <param name="thrower">
        /// The thrower.
        /// </param>
        /// <typeparam name="T">
        /// The exception type.
        /// </typeparam>
        public static void Throw<T>(this InnerThrower thrower) where T : Exception
        {
            if (thrower.IsNotNull())
            {
                Raise<T>();
            }
        }

        /// <summary>
        /// Throw an exception.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type.
        /// </typeparam>
        /// <param name="thrower">
        /// The thrower.
        /// </param>
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public static void Throw<T>(this InnerThrower thrower, string message, params object[] args) where T : Exception
        {
            if (thrower.IsNotNull())
            {
                Raise<T>(message.StringFormat(args));
            }
        }
        
        /// <summary>
        /// Throw an exception.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type.
        /// </typeparam>
        /// <param name="thrower">
        /// The thrower.
        /// </param>
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// <param name="ex">
        /// Inner exception.
        /// </param>
        public static void Throw<T>(this InnerThrower thrower, string message, Exception ex) where T : Exception
        {
            if (thrower.IsNotNull())
            {
                Raise<T>(message, ex);
            }
        }

        /// <summary>
        /// To avoid creating an instance for every condition check we use an inner thrower.
        /// This will only get created if a condition is true and passed to an extension method
        /// </summary>
        public class InnerThrower
        {
        }
    }
}
