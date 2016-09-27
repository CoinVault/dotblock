// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecureStringExtensions.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Blockchain.Protocol.Bitcoin.Security
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Security;

    using Blockchain.Protocol.Bitcoin.Extension;

    /// <summary>
    /// Secure string extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class SecureStringExtensions
    {
        /// <summary>
        /// Converts a string into a secure string.
        /// </summary>
        /// <param name="value">the string to be converted.</param>
        /// <returns>The secure string converted from the input string </returns>
        public static SecureString ConvertToSecureString(this string value)
        {
            Guard.Require(value.IsNotNull());

            SecureString secureString = new SecureString();
            value.ToCharArray().ForEach(secureString.AppendChar);
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Converts the secure string to a string.
        /// </summary>
        /// <param name="secureString">the secure string to be converted.</param> 
        /// <returns>The string converted from a secure string </returns>
        public static string ConvertToString(this SecureString secureString)
        {
            Guard.Require(secureString.IsNotNull());

            IntPtr stringPointer = IntPtr.Zero;
            try
            {
                stringPointer = Marshal.SecureStringToBSTR(secureString);
                return Marshal.PtrToStringBSTR(stringPointer);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(stringPointer);
            }
        }
    }
}
