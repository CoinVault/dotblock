// <copyright file="KeyInfo.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Address
{
    /// <summary>
    /// The address info.
    /// </summary>
    public class KeyInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the private key, the wallet format of a private key.
        /// </summary>
        public string EncodedPrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the encoded public key, effectively this is the bitcoin address.
        /// </summary>
        public string EncodedPublicKey { get; set; }

        #endregion
    }
}