// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionInput.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Transaction.Types
{
    using Blockchain.Protocol.Bitcoin.Transaction.Script;

    /// <summary>
    /// The transaction input.
    /// </summary>
    public class TransactionInput
    {
        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        public long Sequence { get; set; }

        /// <summary>
        /// Gets or sets the script bytes.
        /// </summary>
        public byte[] ScriptBytes { get; set; }

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        public Script ScriptSig { get; set; }

        /// <summary>
        /// Gets or sets the v out.
        /// </summary>
        public TransactionOutPoint Outpoint { get; set; }
    }
}
