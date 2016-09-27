// <copyright file="MnemonicException.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Mnemonic
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// The address format exception.
    /// </summary>
    [Serializable]
    public class MnemonicException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MnemonicException"/> class. 
        /// </summary>
        public MnemonicException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MnemonicException"/> class. 
        /// </summary>
        public MnemonicException(string message)
            : base(message)
        {
        }
    }
}