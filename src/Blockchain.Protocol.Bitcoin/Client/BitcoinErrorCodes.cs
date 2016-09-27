// <copyright file="BitcoinErrorCodes.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Blockchain.Protocol.Bitcoin.Client
{
    /// <summary>
    /// The bit net error codes.
    /// </summary>
    public class BitcoinErrorCodes
    {
        // Bitcoin RPC error codes
        #region Enums

        /// <summary>
        /// The rpc error code.
        /// </summary>
        public enum RpcErrorCode
        {
            //// Standard JSON-RPC 2.0 errors

            /// <summary>
            /// RPC INVALID REQUEST.
            /// </summary>
            RpcInvalidRequest = -32600, 

            /// <summary>
            /// The rpc method not found.
            /// </summary>
            RpcMethodNotFound = -32601, 

            /// <summary>
            /// The rpc  invalid parameters.
            /// </summary>
            RpcInvalidParams = -32602, 

            /// <summary>
            /// The rpc internal error.
            /// </summary>
            RpcInternalError = -32603, 

            /// <summary>
            /// The rpc parse error.
            /// </summary>
            RpcParseError = -32700, 

            //// General application defined errors

            /// <summary>
            /// Exception thrown in command handling
            /// </summary>
            RpcMiscError = -1, 

            /// <summary>
            /// Server is in safe mode, and command is not allowed in safe mode
            /// </summary>
            RpcForbiddenBySafeMode = -2, 

            /// <summary>
            /// Unexpected type was passed as parameter
            /// </summary>
            RpcTypeError = -3, 

            /// <summary>
            /// Invalid address or key
            /// </summary>
            RpcInvalidAddressOrKey = -5, 

            /// <summary>
            /// Ran out of memory during operation
            /// </summary>
            RpcOutOfMemory = -7, 

            /// <summary>
            /// Invalid, missing or duplicate parameter
            /// </summary>
            RpcInvalidParameter = -8, 

            /// <summary>
            /// Database error
            /// </summary>
            RpcDatabaseError = -20, 

            /// <summary>
            /// Error parsing or validating structure in raw format
            /// </summary>
            RpcDeserializationError = -22, 

            //// P2P client errors

            /// <summary>
            /// Bit coin is not connected
            /// </summary>
            RpcClientNotConnected = -9, 

            /// <summary>
            /// Still downloading initial blocks
            /// </summary>
            RpcClientInInitialDownload = -10, 

            /// <summary>
            /// Node is already added
            /// </summary>
            RpcClientNodeAlreadyAdded = -23, 

            /// <summary>
            /// Node has not been added before
            /// </summary>
            RpcClientNodeNotAdded = -24, 

            //// Wallet errors

            /// <summary>
            /// Unspecified problem with wallet (key not found etc.)
            /// </summary>
            RpcWalletError = -4, 

            /// <summary>
            /// Not enough funds in wallet or account
            /// </summary>
            RpcWalletInsufficientFunds = -6, 

            /// <summary>
            /// Invalid account name
            /// </summary>
            RpcWalletInvalidAccountName = -11, 

            /// <summary>
            /// Key pool ran out, call key pool refill first
            /// </summary>
            RpcWalletKeypoolRanOut = -12, 

            /// <summary>
            /// Enter the wallet passphrase with wallet passphrase first
            /// </summary>
            RpcWalletUnlockNeeded = -13, 

            /// <summary>
            /// The wallet passphrase entered was incorrect
            /// </summary>
            RpcWalletPassphraseIncorrect = -14, 

            /// <summary>
            /// Command given in wrong wallet encryption state (encrypting an encrypted wallet etc.)
            /// </summary>
            RpcWalletWrongEncState = -15, 

            /// <summary>
            /// Failed to encrypt the wallet
            /// </summary>
            RpcWalletEncryptionFailed = -16, 

            /// <summary>
            /// Wallet is already unlocked.
            /// </summary>
            RpcWalletAlreadyUnlocked = -17, 
        }

        #endregion
    }
}