// <copyright file="JsonSerializer.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Rosetta.Core.Serialization
{
    #region Using Directives

    using System;

    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// The json serialize.
    /// </summary>
    public class JsonSerializer
    {
        #region Public Methods and Operators

        /// <summary>
        /// The serialize method.
        /// </summary>
        /// <param name="info">
        /// The object to serialize.
        /// </param>
        /// <typeparam name="T">
        /// The object type.
        /// </typeparam>
        /// <returns>
        /// The instance.
        /// </returns>
        public static T DeSerialize<T>(string info) where T : class
        {
            var settings = new JsonSerializerSettings()
                               {
                                   DateTimeZoneHandling = DateTimeZoneHandling.Utc
                               };

            return JsonConvert.DeserializeObject<T>(info, settings);
        }

        /// <summary>
        /// The serialize method.
        /// </summary>
        /// <param name="info">
        /// The object to serialize.
        /// </param>
        /// <typeparam name="T">
        /// The object type.
        /// </typeparam>
        /// <returns>
        /// The instance.
        /// </returns>
        public static T TryDeSerialize<T>(string info) where T : class
        {
            try
            {
                var settings = new JsonSerializerSettings()
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };

                return JsonConvert.DeserializeObject<T>(info, settings);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        /// <summary>
        /// The serialize method.
        /// </summary>
        /// <param name="info">
        /// The object to serialize.
        /// </param>
        /// <typeparam name="T">
        /// The object type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Serialize<T>(T info) where T : class 
        {
            var settings = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            return JsonConvert.SerializeObject(info, settings);
        }

        #endregion
    }
}
