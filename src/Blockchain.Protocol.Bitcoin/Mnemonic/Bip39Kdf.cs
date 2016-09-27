// <copyright file="Bip39Kdf.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591
namespace Blockchain.Protocol.Bitcoin.Mnemonic
{
    #region Using Directives

    using System;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Security.Cryptography;

    #endregion

    /// <summary>
    /// Implementation of the Rfc2898 PBKDF2 specification located here http://www.ietf.org/rfc/rfc2898.txt using HMACSHA512 but modified as opposed to PWDTKto match the BIP39 test vectors
    /// Using BouncyCastle for the HMAC-SHA512 instead of Microsoft implementation
    /// NOTE NOT IDENTICLE TO PWDTK (PWDTK is concatenating password and salt together before hashing the concatenated byte block, this is simply hashing the salt as what we are told to do in BIP39, yes the mnemonic sentence is provided as the hmac key)
    /// Created by thashiznets@yahoo.com.au
    /// </summary>
    public class Bip39Kdf // Rfc2898_pbkdf2_hmacsha512
    {
        #region Private Attributes

        // I made the variable names match the definition in RFC2898 - PBKDF2 where possible, so you can trace the code functionality back to the specification
        /// <summary>
        /// The p.
        /// </summary>
        private readonly byte[] P;

        /// <summary>
        /// The s.
        /// </summary>
        private readonly byte[] S;

        /// <summary>
        /// The c.
        /// </summary>
        private readonly int c;

        /// <summary>
        /// The dk len.
        /// </summary>
        private int dkLen;

        #endregion

        #region Public Constants

        /// <summary>
        /// The c min iterations.
        /// </summary>
        public const int CMinIterations = 2048;

        // Minimum recommended salt length in Rfc2898
        /// <summary>
        /// The c min salt length.
        /// </summary>
        public const int CMinSaltLength = 8;

        // Length of the Hash Digest Output - 512 bits - 64 bytes
        /// <summary>
        /// The h len.
        /// </summary>
        public const int hLen = 64;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Bip39Kdf"/> class. 
        /// Constructor to create Rfc2898_pbkdf2_hmacsha512 object ready to perform Rfc2898 PBKDF2 functionality
        /// </summary>
        /// <param name="password">
        /// The Password to be hashed and is also the HMAC key
        /// </param>
        /// <param name="salt">
        /// Salt to be concatenated with the password
        /// </param>
        /// <param name="iterations">
        /// Number of iterations to perform HMACSHA Hashing for PBKDF2
        /// </param>
        public Bip39Kdf(byte[] password, byte[] salt, int iterations = CMinIterations)
        {
            this.P = password;
            this.S = salt;
            this.c = iterations;
        }

        #endregion

        #region Public Members And Static Methods

        /// <summary>
        /// Derive Key Bytes using PBKDF2 specification listed in Rfc2898 and HMACSHA512 as the underlying PRF (Psuedo Random Function)
        /// </summary>
        /// <param name="keyLength">
        /// Length in Bytes of Derived Key
        /// </param>
        /// <returns>
        /// Derived Key
        /// </returns>
        public byte[] GetDerivedKeyBytes_PBKDF2_HMACSHA512(int keyLength)
        {
            // no need to throw exception for dkLen too long as per spec because dkLen cannot be larger than Int32.MaxValue so not worth the overhead to check
            this.dkLen = keyLength;

            double l = Math.Ceiling((Double)this.dkLen / hLen);

            byte[] finalBlock = new byte[0];

            for (int i = 1; i <= l; i++)
            {
                // Concatenate each block from F into the final block (T_1..T_l)
                finalBlock = finalBlock.Concat(this.F(this.P, this.S, this.c, i)).ToArray();
            }

            // returning DK note r not used as dkLen bytes of the final concatenated block returned rather than <0...r-1> substring of final intermediate block + prior blocks as per spec
            return finalBlock.Take(this.dkLen).ToArray();
        }

        /// <summary>
        /// A static publicly exposed version of GetDerivedKeyBytes_PBKDF2_HMACSHA512 which matches the exact specification in Rfc2898 PBKDF2 using HMACSHA512
        /// </summary>
        /// <param name="P">
        /// Password passed as a Byte Array
        /// </param>
        /// <param name="S">
        /// Salt passed as a Byte Array
        /// </param>
        /// <param name="c">
        /// Iterations to perform the underlying PRF over
        /// </param>
        /// <param name="dkLen">
        /// Length of Bytes to return, an AES 256 key wold require 32 Bytes
        /// </param>
        /// <returns>
        /// Derived Key in Byte Array form ready for use by chosen encryption function
        /// </returns>
        public static byte[] Pbkdf2(byte[] P, byte[] S, int c = CMinIterations, int dkLen = hLen)
        {
            Bip39Kdf rfcObj = new Bip39Kdf(P, S, c);
            return rfcObj.GetDerivedKeyBytes_PBKDF2_HMACSHA512(dkLen);
        }

        #endregion

        #region Private Members

        // Main Function F as defined in Rfc2898 PBKDF2 spec
        /// <summary>
        /// The f.
        /// </summary>
        /// <param name="P">
        /// The p.
        /// </param>
        /// <param name="S">
        /// The s.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The byte.
        /// </returns>
        private byte[] F(byte[] P, byte[] S, int c, int i)
        {
            // Salt and Block number Int(i) concatenated as per spec
            // Byte[] Si = Utilities.MergeByteArrays(S, this.INT(i));
            byte[] Si = S.Concat(this.INT(i)).ToArray();

            // Initial hash (U_1) using password and salt concatenated with Int(i) as per spec
            byte[] temp = this.PRF(Si, P);

            // Output block filled with initial hash value or U_1 as per spec
            byte[] U_c = temp;

            for (int C = 1; C < c; C++)
            {
                // rehashing the password using the previous hash value as salt as per spec
                temp = this.PRF(temp, P);

                for (int j = 0; j < temp.Length; j++)
                {
                    // xor each byte of the each hash block with each byte of the output block as per spec
                    U_c[j] ^= temp[j];
                }
            }

            // return a T_i block for concatenation to create the final block as per spec
            return U_c;
        }

        // PRF function as defined in Rfc2898 PBKDF2 spec
        /// <summary>
        /// The prf.
        /// </summary>
        /// <param name="S">
        /// The s.
        /// </param>
        /// <param name="hmacKey">
        /// The hmac key.
        /// </param>
        /// <returns>
        /// The byte.
        /// </returns>
        private byte[] PRF(byte[] S, byte[] hmacKey)
        {
            // HMACSHA512 Hashing, better than the HMACSHA1 in Microsofts implementation ;)
            return CryptoUtil.ComputeHmac512(hmacKey, S);
        }

        // This method returns the 4 octet encoded Int32 with most significant bit first as per spec
        /// <summary>
        /// The int.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The byte.
        /// </returns>
        private byte[] INT(int i)
        {
            byte[] I = BitConverter.GetBytes(i);

            // Make sure most significant bit is first
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(I);
            }

            return I;
        }

        #endregion
    }
}