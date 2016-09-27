// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="Dark Caesium">
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
    using System.Threading;

    #endregion

    /// <summary>
    /// The exception extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class ExceptionExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The throw if critical.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public static void ThrowIfCritical(this Exception ex)
        {
            if (ex is OutOfMemoryException || ex is ThreadAbortException || ex is StackOverflowException)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Iterates the message path.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The entire collection of messages.</returns>
        public static string ShowAllMessages(this Exception ex)
        {
            // Validate
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            if (ex.InnerException != null)
            {
                return string.Format(CultureInfo.CurrentUICulture, "{0} : {1}", ex.Message, ex.InnerException.ShowAllMessages());
            }

            return ex.Message;
        }

        #endregion
    }
}
