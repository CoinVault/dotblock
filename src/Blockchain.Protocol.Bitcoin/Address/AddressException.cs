// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddressException.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Address
{
    #region

    using System;

    #endregion

    /// <summary>
    /// The address exception.
    /// </summary>
    [Serializable]
    public class AddressException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressException"/> class.
        /// </summary>
        public AddressException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressException"/> class.
        /// </summary>
        public AddressException(string message)
            : base(message)
        {
        }
    }
}