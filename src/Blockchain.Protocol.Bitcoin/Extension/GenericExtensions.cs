// <copyright file="GenericExtensions.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Extension
{
    #region Using Directives

    using System;
    using System.Diagnostics;

    #endregion

    /// <summary>
    /// The generic extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class GenericExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Check if a generic type is of a different type.
        /// </summary>
        /// <param name="obj">
        /// The type to check..
        /// </param>
        /// <typeparam name="T">
        /// The type to check against.
        /// </typeparam>
        /// <returns>
        /// True if types match.
        /// </returns>
        public static T As<T>(this object obj) where T : class 
        {
            return obj as T;
        }

        /// <summary>
        /// Check if a generic type is of a different type.
        /// </summary>
        /// <param name="obj">
        /// The type to check..
        /// </param>
        /// <typeparam name="T">
        /// The type to check against.
        /// </typeparam>
        /// <returns>
        /// True if types match.
        /// </returns>
        public static bool Is<T>(this object obj)
        {
            return obj is T;
        }

        /// <summary>
        /// Check if a generic type is of a different type.
        /// </summary>
        /// <param name="type">
        /// The type to check..
        /// </param>
        /// <typeparam name="T">
        /// The type to check against.
        /// </typeparam>
        /// <returns>
        /// True if types match.
        /// </returns>
        public static bool IsOfType<T>(this Type type)
        {
            return typeof(T) == type;
        }

        /// <summary>
        /// Match type generics.
        /// </summary>
        /// <typeparam name="TA">The first type.</typeparam>
        /// <typeparam name="TB">The second type.</typeparam>
        /// <returns>True if types match.</returns>
        public static bool TypesMatch<TA, TB>()
        {
            return typeof(TA) == typeof(TB);
        }

        /// <summary>
        /// The is assignable from.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <typeparam name="T">
        /// The type to check.
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsAssignableFrom<T>(this Type type)
        {
            return type.IsAssignableFrom(typeof(T));
        }

        /// <summary>
        /// The value or default.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <param name="func">
        /// The delegate.
        /// </param>
        /// <typeparam name="TIn">
        /// Type in.
        /// </typeparam>
        /// <typeparam name="TOut">
        /// Type out.
        /// </typeparam>
        /// <returns>
        /// The returned class.
        /// </returns>
        public static TOut ValueOrDefault<TIn, TOut>(this TIn obj, Func<TIn, TOut> func) where TIn : class 
        {
            return obj.IsNotNull() ? func(obj) : default(TOut);
        }

        #endregion
    }
}
