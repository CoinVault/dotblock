// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionOutput.cs" company="Dark Caesium">
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
    public class TransactionOutput
    {
        /// <summary>
        /// Gets or sets the script bytes.
        /// </summary>
        public byte[] ScriptBytes { get; set; }

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        public Script ScriptPubKey { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// Gets or sets the n.
        /// </summary>
        public long Index { get; set; }
    }
}
