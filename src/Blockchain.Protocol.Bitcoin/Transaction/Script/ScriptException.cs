// <copyright file="ScriptException.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Transaction.Script
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// The transaction exception.
    /// </summary>
    public class ScriptException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        public ScriptException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        public ScriptException(string message)
            : base(message)
        {
        }
    }
}
