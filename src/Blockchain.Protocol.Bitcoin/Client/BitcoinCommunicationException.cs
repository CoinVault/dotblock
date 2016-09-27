// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitcoinCommunicationException.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Client
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// The client communication exception.
    /// </summary>
    public class BitcoinCommunicationException : ApplicationException
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitcoinCommunicationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="ex">
        /// The inner exception.
        /// </param>
        public BitcoinCommunicationException(string message, Exception ex)
            : base(message, ex)
        {
        }

        #endregion
    }
}
