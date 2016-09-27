// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendInfo.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Transaction.Types
{
    /// <summary>
    /// The address send info.
    /// </summary>
    public class SendInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether take fee from this address.
        /// </summary>
        public bool TakeFee { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="publicKey">
        /// The public key.
        /// </param>
        /// <returns>
        /// The <see cref="SendInfo"/>.
        /// </returns>
        public static SendInfo Create(decimal amount, string publicKey)
        {
            return new SendInfo { Amount = amount, PublicKey = publicKey };
        }

        #endregion
    }
}