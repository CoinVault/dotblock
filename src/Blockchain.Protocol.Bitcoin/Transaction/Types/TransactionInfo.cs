// <copyright file="TransactionInfo.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Transaction.Types
{
    #region Using Directives

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Used to hold state of a transaction creation processes.
    /// </summary>
    public class TransactionInfo
    {
        /// <summary>
        /// Gets or sets the spend from address.
        /// </summary>
        public List<ReceiveInfo> SpendFromAddresses { get; set; }

        /// <summary>
        /// Gets or sets the spend to addresses.
        /// </summary>
        public List<SendInfo> SpendToAddresses { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string ChangeAddress { get; set; }

        /// <summary>
        /// Gets or sets the fee.
        /// </summary>
        public decimal RequestedFee { get; set; }

        /// <summary>
        /// Gets or sets the fee.
        /// </summary>
        public decimal CoinFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction failed.
        /// </summary>
        public bool Failed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction failed.
        /// </summary>
        public bool CanRetry { get; set; }

        /// <summary>
        /// Gets or sets failed message.
        /// </summary>
        public FailedReason FailedReason { get; set; }

        /// <summary>
        /// The fail.
        /// </summary>
        /// <param name="retry">
        /// The retry.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Fail(bool retry, FailedReason reason)
        {
            this.Failed = true;
            this.CanRetry = retry;
            this.FailedReason = reason;
        }
    }
}