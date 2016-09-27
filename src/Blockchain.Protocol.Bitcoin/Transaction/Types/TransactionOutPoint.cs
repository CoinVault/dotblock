// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionOutPoint.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Transaction.Types
{
    /// <summary>
    /// The transaction input.
    /// </summary>
    public class TransactionOutPoint
    {
        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the v out.
        /// </summary>
        public long Index { get; set; }
    }
}
