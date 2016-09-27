// <copyright file="CreateRawTransaction.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Client.Types
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    /// Create a raw transaction.
    /// </summary>
    public class CreateRawTransaction
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRawTransaction"/> class.
        /// </summary>
        public CreateRawTransaction()
        {
            this.Inputs = new List<CreateRawTransactionInput>();
            this.Outputs = new Dictionary<string, decimal>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the inputs.
        /// </summary>
        public List<CreateRawTransactionInput> Inputs { get; set; }

        /// <summary>
        /// Gets or sets a dictionary with the output address and amount per address.
        /// </summary>
        public Dictionary<string, decimal> Outputs { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add input.
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <returns>
        /// The <see cref="CreateRawTransaction"/>.
        /// </returns>
        public CreateRawTransaction AddInput(string transactionId, int output)
        {
            this.Inputs.Add(new CreateRawTransactionInput { TransactionId = transactionId, Output = output });
            return this;
        }

        /// <summary>
        /// The add output.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <returns>
        /// The <see cref="CreateRawTransaction"/>.
        /// </returns>
        public CreateRawTransaction AddOutput(string address, decimal amount)
        {
            this.Outputs.Add(address, amount);
            return this;
        }

        /// <summary>
        /// The reduce fee from last output.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="fee">
        /// The fee.
        /// </param>
        /// <returns>
        /// The <see cref="CreateRawTransaction"/>.
        /// </returns>
        public CreateRawTransaction ReduceFeeFromAddress(string address, decimal fee)
        {
            var output = this.Outputs.First(f => f.Key == address);
            var amont = output.Value;
            var final = amont - fee;

            if (final < 0)
            {
                throw new BitcoinClientException("Invalid fee");
            }

            if (final == 0)
            {
                this.Outputs.Remove(output.Key);
                return this;
            }

            this.Outputs[output.Key] = amont - fee;
            return this;
        }

        #endregion
    }
}
