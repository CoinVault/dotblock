// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitcoinClientFactory.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Client
{
    #region Using Directives

    using System;
    using System.Runtime.Caching;

    using Blockchain.Protocol.Bitcoin.Extension;

    #endregion

    /// <summary>
    ///  Client factory that will reuse client instances based on given parameters.
    /// </summary>
    public class BitcoinClientFactory
    {
        #region Static Fields

        /// <summary>
        ///     Defines a cache object to hold storage sources.
        /// </summary>
        private static readonly MemoryCache Cache = new MemoryCache("BitcoinClients");

        /// <summary>
        ///     Defines a lock object for the cache.
        /// </summary>
        private static readonly object CacheLock = new object();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// A static method to create a client.
        /// </summary>
        /// <param name="connection">
        /// The connection ip or host.
        /// </param>
        /// <param name="port">
        /// The port the client rpc is listening on.
        /// </param>
        /// <param name="user">
        /// The user name.
        /// </param>
        /// <param name="encPass">
        /// The encrypted password.
        /// </param>
        /// <param name="secure">
        /// Indicator to use ssl (https).
        /// </param>
        /// <returns>
        /// A new instance of <see cref="BitcoinClient"/>.
        /// </returns>
        public static IBitcoinClient Create(string connection, int port, string user, string encPass, bool secure)
        {
            // Set cache key name
            var cacheKey = "{0}:{1}:{2}:{3}".StringFormat(connection, port, user, secure);

            // Get storage source from cache if available
            var client = (IBitcoinClient)Cache.Get(cacheKey);
            if (client == null)
            {
                // Assuming no storage source in cache, attempt to create it
                lock (CacheLock)
                {
                    client = (IBitcoinClient)Cache.Get(cacheKey);
                    if (client == null)
                    {
                        // Add to cache
                        client = BitcoinClient.Create(connection, port, user, encPass, secure);
                        Cache.Add(cacheKey, client, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 60, 0) });
                    }
                }
            }

            // Return storage source
            return client;
        }

        #endregion
    }
}
