// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionContext.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Transaction.Types
{
    #region Using Directives

    using System.Collections.Generic;

    using Blockchain.Protocol.Bitcoin.Client.Types;
    using Blockchain.Protocol.Bitcoin.Common;

    #endregion

    /// <summary>
    /// The transaction context is used to build a transaction, 
    /// it holds information required in the building processes like private keys and public script hash.
    /// </summary>
    public class TransactionContext
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the created raw transaction.
        /// </summary>
        public CreateRawTransaction CreateRawTransaction { get; set; }

        /// <summary>
        /// Gets or sets the decoded raw transaction.
        /// </summary>
        public DecodedRawTransaction DecodedRawTransaction { get; set; }

        /// <summary>
        /// Gets or sets the create raw transaction.
        /// </summary>
        public List<DecodedRawTransaction> PreviousRawTransactions { get; set; }

        /// <summary>
        /// Gets or sets the sign raw transaction information.
        /// </summary>
        public SignRawTransaction SignRawTransaction { get; set; }

        /// <summary>
        /// Gets or sets the signed raw transaction.
        /// </summary>
        public SignedRawTransaction SignedRawTransaction { get; set; }

        /// <summary>
        /// Gets or sets the SendItems.
        /// </summary>
        public List<TransactionInfo> SendItems { get; set; }

        /// <summary>
        /// Gets or sets the unsigned raw transaction.
        /// </summary>
        public string UnsignedRawTransaction { get; set; }

        /// <summary>
        /// Gets or sets the unsigned transaction.
        /// </summary>
        public DecodedRawTransaction UnsignedTransaction { get; set; }

        /// <summary>
        /// Gets or sets the coin parameters.
        /// </summary>
        public CoinParameters CoinParameters { get; set; }

        #endregion
    }
}