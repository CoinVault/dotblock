// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceiveInfo.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Transaction.Types
{
    #region Using Directives

    using System.Collections.Generic;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Common;

    #endregion

    /// <summary>
    /// The address send info.
    /// </summary>
    public class ReceiveInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the private key.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the transactions.
        /// </summary>
        public List<ReceiveInfoTransaction> Transactions { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="privateKey">
        /// The private key.
        /// </param>
        /// <param name="publicKey">
        /// The public key.
        /// </param>
        /// <returns>
        /// The <see cref="ReceiveInfo"/>.
        /// </returns>
        public static ReceiveInfo Create(string privateKey, string publicKey)
        {
            return new ReceiveInfo { PrivateKey = privateKey, PublicKey = publicKey };
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="headers">
        /// The headers.
        /// </param>
        /// <returns>
        /// The <see cref="ReceiveInfo"/>.
        /// </returns>
        public BitcoinPrivateKey GetKey(CoinParameters headers)
        {
            return new BitcoinPrivateKey(headers, this.PrivateKey);
        }

        #endregion
    }
}