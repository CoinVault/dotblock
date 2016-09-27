// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecodedRawTransaction.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   The decoded raw transaction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Client.Types
{
    #region Using Directives

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// The decoded raw transaction.
    /// </summary>
    public class DecodedRawTransaction
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the hex.
        /// </summary>
        public string Hex { get; set; }

        /// <summary>
        /// Gets or sets the lock time.
        /// </summary>
        public long Locktime { get; set; }

        /// <summary>
        /// Gets or sets the time (used by POS clients).
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        public string TxId { get; set; }

        /// <summary>
        /// Gets or sets the v in.
        /// </summary>
        public List<Vin> VIn { get; set; }

        /// <summary>
        /// Gets or sets the v out.
        /// </summary>
        public List<Vout> VOut { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public int Version { get; set; }

        #endregion
    }
}
