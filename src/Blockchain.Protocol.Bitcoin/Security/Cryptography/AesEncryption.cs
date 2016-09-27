// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AesEncryption.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   This class provides AES encryption helper methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Security.Cryptography
{
    #region Using Directives

    using System;
    using System.IO;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;

    #endregion

    /// <summary>
    /// This class provides AES encryption helper methods.
    /// </summary>
    public class AesEncryption
    {
        #region Public Methods and Operators

        /// <summary>
        /// The decrypt string from bytes AES.
        /// </summary>
        /// <param name="cipherText">
        /// The cipher text.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="initializationVector">
        /// The initialization vector.
        /// </param>
        /// <returns>
        /// The decrypted string.
        /// </returns>
        public static string DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] initializationVector)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (initializationVector == null || initializationVector.Length <= 0)
            {
                throw new ArgumentNullException(nameof(initializationVector));
            }

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = initializationVector;

                // Create a decryption object to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption. 
                using (var msdecrypt = new MemoryStream(cipherText))
                {
                    using (var csdecrypt = new CryptoStream(msdecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srdecrypt = new StreamReader(csdecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srdecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        /// <summary>
        /// The encrypt string to bytes AES.
        /// </summary>
        /// <param name="plainText">
        /// The plain text.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="initializationVector">
        /// The initialization vector.
        /// </param>
        /// <returns>
        /// The encrypted string.
        /// </returns>
        public static byte[] EncryptStringToBytesAes(string plainText, byte[] key, byte[] initializationVector)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(plainText));
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (initializationVector == null || initializationVector.Length <= 0)
            {
                throw new ArgumentNullException(nameof(initializationVector));
            }

            byte[] encrypted;

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = initializationVector;

                // Create a n encryption object to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (var msencrypt = new MemoryStream())
                {
                    using (var csencrypt = new CryptoStream(msencrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swencrypt = new StreamWriter(csencrypt))
                        {
                            // Write all data to the stream.
                            swencrypt.Write(plainText);
                        }

                        encrypted = msencrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream. 
            return encrypted;
        }

        /// <summary>
        /// The decrypt string from bytes AES.
        /// </summary>
        /// <param name="cipherText">
        /// The cipher text.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="initializationVector">
        /// The initialization vector.
        /// </param>
        /// <returns>
        /// The decrypted string.
        /// </returns>
        public static SecureString DecryptSecureStringFromBytesAes(byte[] cipherText, byte[] key, byte[] initializationVector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The encrypt string to bytes AES.
        /// </summary>
        /// <param name="plainText">
        /// The plain text.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="initializationVector">
        /// The initialization vector.
        /// </param>
        /// <returns>
        /// The encrypted string.
        /// </returns>
        public static byte[] EncryptSecureStringToBytesAes(SecureString plainText, byte[] key, byte[] initializationVector)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}