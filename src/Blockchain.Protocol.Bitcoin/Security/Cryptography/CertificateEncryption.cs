// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificateEncryption.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   Encryption functionality for Certificates.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Blockchain.Protocol.Bitcoin.Security.Cryptography
{
    #region Using Statements

    using System;
    using System.Collections;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    #endregion

    /// <summary>
    /// Encrypt and Decrypt data using x509 certificates.
    /// </summary>
    public static class CertificateEncryption
    {
        #region Internal Static Methods

        /// <summary>
        /// Encrypt a single string value using the certificate.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="certificate">The Certificate.</param>
        /// <returns>The base64 encoded encrypted field value.</returns>
        public static string EncryptString(string fieldValue, X509Certificate2 certificate)
        {
            // Validate
            if (fieldValue == null)
            {
                throw new ArgumentNullException(nameof(fieldValue));
            }

            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }
           
            // Create a new instance of RSACryptoServiceProvider using the Public Key from Certificate.
            RSACryptoServiceProvider rsaAlg = certificate.PublicKey.Key as RSACryptoServiceProvider;
            if (rsaAlg == null)
            {
                throw new ApplicationException(string.Format(CultureInfo.CurrentUICulture, "Certificate {0} Does Not Have Public Key", certificate.Subject));
            }

            // Retrieve the sensitive data
            UnicodeEncoding uniEncoding = new UnicodeEncoding();
            byte[] dataToEncrypt = uniEncoding.GetBytes(fieldValue);

            // Work out buffering parameters as RSA only supports a limited size out the box
            int keySize = rsaAlg.KeySize / 8;
            int maxSupportedBufferLength = keySize - 42;
            int dataLength = dataToEncrypt.Length;
            int bufferCount = dataLength / maxSupportedBufferLength;
            var stringBuilder = new StringBuilder();

            // Process Buffers 
            for (int i = 0; i <= bufferCount; i++)
            {
                int byteCount = dataLength - (maxSupportedBufferLength * i) > maxSupportedBufferLength
                        ? maxSupportedBufferLength
                        : dataLength - (maxSupportedBufferLength * i);

                var buffer = new byte[byteCount];

                Buffer.BlockCopy(dataToEncrypt, maxSupportedBufferLength * i, buffer, 0, buffer.Length);

                byte[] encryptedBytes = rsaAlg.Encrypt(buffer, false);

                Array.Reverse(encryptedBytes);
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Decrypt a single string value using the certificate.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="certificate">The Certificate.</param>
        /// <returns>The decrypted field value.</returns>
        public static string DecryptString(string fieldValue, X509Certificate2 certificate)
        {
            // Validate
            if (fieldValue == null)
            {
                throw new ArgumentNullException(nameof(fieldValue));
            }

            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            // Create a new instance of RSACryptoServiceProvider using the 
            // key from RSAParameters.
            RSACryptoServiceProvider rsaAlg = certificate.PrivateKey as RSACryptoServiceProvider;
            if (rsaAlg == null)
            {
                throw new ApplicationException(string.Format(CultureInfo.CurrentUICulture, "Certificate {0} Does Not Have Private Key", certificate.Subject));
            }

            // Work out buffering parameters as RSA only supports a limited size out the box
            int keySize = rsaAlg.KeySize;
            int blockSize = ((keySize / 8) % 3 != 0)
                                        ? (((keySize / 8) / 3) * 4) + 4
                                        : ((keySize / 8) / 3) * 4;

            int bufferCount = fieldValue.Length / blockSize;
            var arrayList = new ArrayList();

            // Process Buffers 
            for (int i = 0; i < bufferCount; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(
                    fieldValue.Substring(blockSize * i, blockSize));

                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaAlg.Decrypt(encryptedBytes, false));
            }

            UnicodeEncoding uniEncoding = new UnicodeEncoding();

            return uniEncoding.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }

        #endregion
    }
}
