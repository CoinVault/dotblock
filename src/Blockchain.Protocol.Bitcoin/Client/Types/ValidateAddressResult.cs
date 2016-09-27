// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateAddressResult.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   The validate address result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Client.Types
{
    /// <summary>
    /// The validate address result.
    /// </summary>
    public class ValidateAddressResult
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is valid.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is mine.
        /// </summary>
        public bool IsMine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is script.
        /// </summary>
        public bool IsScript { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is valid.
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether is valid.
        /// </summary>
        public string Pubkey { get; set; }

        #endregion
    }
}
