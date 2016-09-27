// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBitcoinClient.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Client
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Blockchain.Protocol.Bitcoin.Client.Types;

    #endregion

    /// <summary>
    /// The BitcoinClient interface.
    /// </summary>
    public interface IBitcoinClient
    {
        #region Public Methods and Operators

        /// <summary>
        /// Safely copies wallet file to destination, which can be a directory or a path with filename.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task BackupWalletAsync(string destination);

        /// <summary>
        /// Version 0.7: Creates a raw transaction spending given inputs.
        /// </summary>
        /// <param name="rawTransaction">
        /// The raw transaction details.
        /// </param>
        /// <returns>
        /// The raw transaction hex. The transaction is not signed yet.
        /// </returns>
        Task<string> CreateRawTransactionAsync(CreateRawTransaction rawTransaction);

        /// <summary>
        /// Version 0.7: Produces a human-readable JSON object for a raw transaction.
        /// </summary>
        /// <param name="rawTransactionHex">
        /// The hex of the raw transaction.
        /// </param>
        /// <returns>
        /// The decoded raw transaction details.
        /// </returns>
        Task<DecodedRawTransaction> DecodeRawTransactionAsync(string rawTransactionHex);

        /// <summary>
        /// Returns the current bitcoin address for receiving payments to this account.
        /// </summary>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        Task<string> GetAccountAddressAsync(string account);

        /// <summary>
        /// Returns the account associated with the given address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The account.
        /// </returns>
        Task<string> GetAccountAsync(string address);

        /// <summary>
        /// Returns the list of addresses for the given account.
        /// </summary>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <returns>
        /// The addresses.
        /// </returns>
        Task<IEnumerable<string>> GetAddressesByAccountAsync(string account);

        /// <summary>
        /// If [account] is not specified, returns the server's total available balance.
        /// If [account] is specified, returns the balance in the account.
        /// </summary>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <param name="minconf">
        /// The confirmations.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        Task<decimal> GetBalanceAsync(string account = "", int minconf = 1);

        /// <summary>
        /// Get a block.
        /// </summary>
        /// <param name="hash">
        /// The block hash.
        /// </param>
        /// <returns>
        /// The block.
        /// </returns>
        Task<BlockInfo> GetBlockAsync(string hash);

        /// <summary>
        /// Returns the number of blocks in the longest block chain.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        Task<int> GetBlockCountAsync();

        /// <summary>
        /// Returns the number of connections to other nodes.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        Task<int> GetConnectionCountAsync();

        /// <summary>
        /// Returns the proof-of-work difficulty as a multiple of the minimum difficulty.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        Task<decimal> GetDifficultyAsync();

        /// <summary>
        /// Returns true or false whether bitcoin demon is currently generating hashes
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        Task<bool> GetGenerateAsync();

        /// <summary>
        /// Returns a recent hashes per second performance measurement while generating.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        Task<decimal> GetHashesPerSecAsync();

        /// <summary>
        /// Returns an object containing various state info.
        /// </summary>
        /// <returns>
        /// The <see cref="ClientInfo"/>.
        /// </returns>
        Task<ClientInfo> GetInfoAsync();

        /// <summary>
        /// Returns a new bitcoin address for receiving payments.
        /// If [account] is specified (recommended), it is added to the address book so payments 
        /// received with the address will be credited to [account].
        /// </summary>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        Task<string> GetNewAddressAsync(string account);

        /// <summary>
        /// Get the current (unconfirmed) transactions from memory pool.
        /// </summary>
        /// <returns>
        /// Collection of transactions.
        /// </returns>
        Task<IEnumerable<string>> GetRawMemPoolAsync();

        /// <summary>
        /// Version 0.7: Returns raw transaction representation for given transaction id. 
        /// </summary>
        /// <param name="txid">
        /// The transaction id.
        /// </param>
        /// <param name="verbose">
        /// The verbosity level. If it is higher than 0, 
        /// a lot more information will be returned.
        /// </param>
        /// <returns>
        /// The raw transaction hex.
        /// </returns>
        Task<DecodedRawTransaction> GetRawTransactionAsync(string txid, int verbose = 0);

        /// <summary>
        /// Returns the total amount received by addresses with account in 
        /// transactions with at least confirmations.
        /// </summary>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <param name="minconf">
        /// The confirmations.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        Task<decimal> GetReceivedByAccountAsync(string account, int minconf = 1);

        /// <summary>
        /// Returns the total amount received by bitcoin address in transactions 
        /// with at least confirmations.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="minconf">
        /// The confirmations.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        Task<decimal> GetReceivedByAddressAsync(string address, int minconf = 1);

        /// <summary>
        /// Get detailed information about the transaction id
        /// </summary>
        /// <param name="txid">
        /// The transaction id
        /// </param>
        /// <returns>
        /// The <see cref="TransactionInfo"/>.
        /// </returns>
        Task<TransactionInfo> GetTransactionAsync(string txid);

        /// <summary>
        /// Get detailed information about a transaction id not in the wallet
        /// </summary>
        /// <param name="txid">
        /// The transaction id
        /// </param>
        /// <param name="outputIndex">
        /// The output index number.
        /// </param>
        /// <param name="includemempool">
        /// The optional to look in the memory pool.
        /// </param>
        /// <returns>
        /// The <see cref="TransactionInfo"/>.
        /// </returns>
        Task<TransactionOutputInfo> GetTxOutAsync(string txid, int outputIndex, bool includemempool = true);

        /// <summary>
        ///  Returns formatted hash data to work on.
        /// </summary>
        /// <returns>
        /// The <see cref="WorkInfo"/>.
        /// </returns>
        Task<WorkInfo> GetWorkAsync();

        /// <summary>
        /// Tries to solve the block and returns true if it was successful
        /// </summary>
        /// <param name="data">
        /// The work data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        Task<bool> GetWorkAsync(string data);

        /// <summary>
        /// Dumps the block existing at specified height. 
        /// </summary>
        /// <param name="index">
        /// The block index in the chain.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        Task<string> GetblockHashAsync(long index);

        /// <summary>
        /// List commands, or get help for a command.
        /// </summary>
        /// <param name="command">
        /// The help command.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        Task<string> HelpAsync(string command = "");

        /// <summary>
        /// Returns Object that has account names as keys, account balances as values.
        /// </summary>
        /// <param name="minconf">
        /// The confirmations.
        /// </param>
        /// <returns>
        /// The collection of <see cref="TransactionAccountInfo"/>.
        /// </returns>
        Task<IEnumerable<TransactionAccountInfo>> ListAccountsAsync(int minconf = 1);

        /// <summary>
        /// Returns an array of objects containing:
        /// </summary>
        /// <param name="minconf">
        /// The confirmations.
        /// </param>
        /// <param name="includeEmpty">
        /// Include empty accounts.
        /// </param>
        /// <returns>
        /// The collection of <see cref="TransactionInfo"/>.
        /// </returns>
        Task<IEnumerable<TransactionInfo>> ListReceivedByAccountAsync(int minconf = 1, bool includeEmpty = false);

        /// <summary>
        /// Returns an array of objects containing:
        /// </summary>
        /// <param name="minconf">
        /// The confirmations.
        /// </param>
        /// <param name="includeEmpty">
        /// Include empty accounts.
        /// </param>
        /// <returns>
        /// The collection of <see cref="TransactionInfo"/>.
        /// </returns>
        Task<IEnumerable<TransactionInfo>> ListReceivedByAddressAsync(int minconf = 1, bool includeEmpty = false);

        /// <summary>
        /// Returns up to [count] most recent transactions for account account.
        /// </summary>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <param name="count">
        /// Transaction count.
        /// </param>
        /// <returns>
        /// The collection of <see cref="TransactionAccountInfo"/>.
        /// </returns>
        Task<IEnumerable<TransactionAccountInfo>> ListTransactionsAsync(string account, int count = 10);

        /// <summary>
        /// Move from one account in your wallet to another.
        /// </summary>
        /// <param name="fromAccount">
        /// Send from.
        /// </param>
        /// <param name="toAccount">
        /// Send to.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="minconf">
        /// The confirmations.
        /// </param>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        Task<bool> MoveAsync(string fromAccount, string toAccount, decimal amount, int minconf = 1, string comment = "");

        /// <summary>
        /// Amount is a real and is rounded to the nearest 0.01. Returns the transaction ID if successful.
        /// </summary>
        /// <param name="fromAccount">
        /// Send from account.
        /// </param>
        /// <param name="toAddress">
        /// Send to address.
        /// </param>
        /// <param name="amount">
        /// The amount to send.
        /// </param>
        /// <param name="minconf">
        /// The minimum confirmations the account has to have.
        /// </param>
        /// <param name="comment">
        /// A comment to write to the block chain.
        /// </param>
        /// <param name="commentTo">
        /// A comment to write to the block chain on the sender.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        Task<string> SendFromAsync(string fromAccount, string toAddress, decimal amount, int minconf = 1, string comment = "", string commentTo = "");

        /// <summary>
        /// Send coins to address. Returns transaction id.
        /// </summary>
        /// <param name="address">
        /// The address to send to.
        /// </param>
        /// <param name="amount">
        /// The amount to send.
        /// </param>
        /// <param name="comment">
        /// A comment to write to the block chain.
        /// </param>
        /// <param name="commentTo">
        /// A comment to write to the block chain on the sender.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        Task<string> SendToAddressAsync(string address, decimal amount, string comment, string commentTo);

        /// <summary>
        /// Version 0.7: Submits raw transaction (serialized, hex-encoded) to local node and network.
        /// </summary>
        /// <param name="hexString">
        /// The hex String.
        /// </param>
        /// <returns>
        /// Submits raw transaction (serialized, hex-encoded) to local node and network. Returns transaction id, or an error if the transaction is invalid for any reason.
        /// </returns>
        Task<string> SentRawTransactionAsync(string hexString);

        /// <summary>
        /// Sets the account associated with the given address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task SetAccountAsync(string address, string account);

        /// <summary>
        /// Generation is limited to processors, -1 is unlimited.
        /// </summary>
        /// <param name="generate">
        /// true or false to turn generation on or off
        /// </param>
        /// <param name="genproclimit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task SetGenerateAsync(bool generate, int genproclimit = 1);

        /// <summary>
        /// The sign raw transaction async.
        /// </summary>
        /// <param name="rawTransaction">
        /// The raw transaction.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<SignedRawTransaction> SignRawTransactionAsync(SignRawTransaction rawTransaction);

        /// <summary>
        /// Stop bitcoin server.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task StopAsync();

        /// <summary>
        /// Return information about bitcoin address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="ValidateAddressResult"/>.
        /// </returns>
        Task<ValidateAddressResult> ValidateAddressAsync(string address);

        #endregion
    }
}