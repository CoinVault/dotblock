// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRpcError.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   The json rpc error.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Client.Types
{
    #region Using Directives

    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// The json rpc error.
    /// </summary>
    public class JsonRpcError
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        [JsonProperty(PropertyName = "code", Order = 0)]
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonProperty(PropertyName = "message", Order = 1)]
        public string Message { get; set; }

        #endregion
    }
}
