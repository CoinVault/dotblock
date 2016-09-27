// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vin.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Client.Types
{
    /// <summary>
    /// The in.
    /// </summary>
    public class Vin
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the script sig.
        /// </summary>
        public ScriptSig ScriptSig { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        public long Sequence { get; set; }

        /// <summary>
        /// Gets or sets the coin-base.
        /// </summary>
        public string CoinBase { get; set; }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        public string TxId { get; set; }

        /// <summary>
        /// Gets or sets the v out.
        /// </summary>
        public int VOut { get; set; }

        #endregion
    }
}