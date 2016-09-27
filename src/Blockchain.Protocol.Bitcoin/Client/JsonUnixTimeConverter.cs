// <copyright file="JsonUnixTimeConverter.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Client
{
    #region Using Directives

    using System;

    using Blockchain.Protocol.Bitcoin.Extension;

    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// Convert a Unix time either from numeric representation of a string representation (PPC).
    /// </summary>
    public class JsonUnixTimeConverter : JsonConverter
    {
        #region Public Methods and Operators

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            long ret = 0;
            if (long.TryParse(reader.Value.ToString(), out ret))
            {
                return ret;
            }

            DateTime dt;
            if (DateTime.TryParse(reader.Value.ToString(), out dt))
            {
                return dt.UnixTimeStampFromDateTime();
            }

            return ret;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
