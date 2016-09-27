// <copyright file="ExtendedKeyPath.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Address
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Blockchain.Protocol.Bitcoin.Extension;

    #endregion

    /// <summary>
    /// Represents the path of an extended key.
    /// </summary>
    public class ExtendedKeyPath
    {
        #region Public Properties

        public List<uint> Items { get; set; }

        public string Path { get; set; }

        public static ExtendedKeyPath Parse(string path)
        {
            Guard.Require(path.StartsWith("m/"));

            return new ExtendedKeyPath { Path = path, Items = path.Substring(2).Split('/').Select(ConvertPathItem).ToList() };
        }

        public uint Index(int index)
        {
            return this.Items.ElementAt(index);
        }

        public ExtendedKeyPath AddChild(string child)
        {
            this.Items.Add(ConvertPathItem(child));
            return this;
        }

        public ExtendedKeyPath AddChild(uint child)
        {
            this.Items.Add(child);
            return this;
        }

        public bool IsHardendIndex(int index)
        {
            return ExtendedKey.IsHardendIndex(this.Items.ElementAt(index));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("m/");

            this.Items.Aggregate(
                builder, 
                (current, item) => 
                current.Append("{0}{1}/".StringFormat(
                ExtendedKey.IsHardendIndex(item) ? ExtendedKey.FromHadrendIndex(item) : item, 
                ExtendedKey.IsHardendIndex(item) ? "'" : string.Empty)));

            return builder.ToString().TrimEnd("/".ToCharArray());
        }

        protected static uint ConvertPathItem(string item)
        {
            return item.Contains("'") ? ExtendedKey.ToHadrendIndex(uint.Parse(item.TrimEnd("'".ToCharArray()))) : uint.Parse(item);
        }

        #endregion
    }
}