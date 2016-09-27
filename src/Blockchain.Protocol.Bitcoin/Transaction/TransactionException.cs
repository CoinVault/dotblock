// <copyright file="TransactionException.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Transaction
{
    #region Using Directives

    using System;

    #endregion

    public class TransactionException : Exception
    {
        public TransactionException()
        {
        }

        public TransactionException(string message)
            : base(message)
        {
        }

        public TransactionException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
