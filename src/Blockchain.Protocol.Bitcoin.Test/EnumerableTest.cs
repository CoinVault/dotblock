// <copyright file="EnumerableTest.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Test
{
    #region Using Directives

    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Extension;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    /// <summary>
    /// </summary>
    [TestClass]
    public class EnumerableTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        [TestMethod]
        public void TestBatchEnumeration()
        {
            var range = Enumerable.Range(1, 20);

            var batched = range.Batch(5);

            batched.ForEach(b => Assert.AreEqual(5, b.Count()));
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void TestBatchEnumerationUnEven()
        {
            var range = Enumerable.Range(1, 22);

            var batched = range.Batch(5);

            batched.ForEach((b, i) => Assert.AreEqual(i == 4 ? 2 : 5, b.Count()));
        }

        #endregion
    }
}
