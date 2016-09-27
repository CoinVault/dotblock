// <copyright file="FailedReason.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Transaction.Types
{
    /// <summary>
    /// The multi send failure reason.
    /// </summary>
    public enum FailedReason
    {
        /// <summary>
        /// No failure.
        /// </summary>
        None, 

        /// <summary>
        /// Invalid sum.
        /// </summary>
        InvalidSum, 

        /// <summary>
        /// The insufficient fee on sender.
        /// </summary>
        InsufficientFeeOnSender, 

        /// <summary>
        /// The insufficient fee on sender.
        /// </summary>
        InsufficientFeeOnReceiver, 

        /// <summary>
        /// Fee Cannot Be Zero.
        /// </summary>
        FeeCannotBeZero, 

        /// <summary>
        /// Fee Cannot Be Zero.
        /// </summary>
        ReceiveAddressAlreadyInOutput, 

        /// <summary>
        /// Fee Cannot Be Zero.
        /// </summary>
        TransactionInputAlreadyInUse, 

        /// <summary>
        /// Fee Cannot Be Zero.
        /// </summary>
        NoTransactionsFound, 

        /// <summary>
        /// Failed reading from client.
        /// </summary>
        FailedReadingTransactionsFromClient,

        /// <summary>
        /// Failed reading from client.
        /// </summary>
        ScriptPubKeyHexNotFound, 

        /// <summary>
        /// The cannot send to self unless change.
        /// </summary>
        CannotSendToSelfUnlessChange,

        /// <summary>
        /// The cannot send to self unless change.
        /// </summary>
        NoChangeAddressFound,

        /// <summary>
        /// The insufficient fee on sender.
        /// </summary>
        InsufficientFeeForTransactionSize, 

    }
}