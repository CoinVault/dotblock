// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignedRawTransaction.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   The signed raw transaction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Client.Types
{
    /// <summary>
    /// The signed raw transaction.
    /// </summary>
    public class SignedRawTransaction
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether complete.
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// Gets or sets the hex.
        /// </summary>
        public string Hex { get; set; }

        #endregion
    }
}
