// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionSignature.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591
namespace Blockchain.Protocol.Bitcoin.Transaction
{
    #region Using Directives

    using System;
    using System.Linq;

    using Blockchain.Protocol.Bitcoin.Address;
    using Blockchain.Protocol.Bitcoin.Transaction.Types;

    using Org.BouncyCastle.Math;

    #endregion

    /// <summary>
    /// The transaction signature.
    /// </summary>
    public class TransactionSignature : EcKey.EcdsaSignature
    {
        /// <summary>
        /// The signature hash flags.
        /// </summary>
        private readonly Transaction.SigHash sighash;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionSignature"/> class.
        /// </summary>
        public TransactionSignature(BigInteger r, BigInteger s)
            : this(r, s, Transaction.SigHash.All)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionSignature"/> class.
        /// </summary>
        public TransactionSignature(BigInteger r, BigInteger s, Transaction.SigHash sighash)
            : base(r, s)
        {

            this.sighash = sighash;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionSignature"/> class.
        /// </summary>
        public TransactionSignature(EcKey.EcdsaSignature signature, Transaction.SigHash mode)
            : base(signature.R, signature.S)
        {
            this.sighash = mode;
        }

        /// <summary>
        /// Returns a dummy invalid signature whose R/S values are set such that they will take up the same number of
        ///  encoded bytes as a real signature. This can be useful when you want to fill out a transaction to be of the
        ///  right size (e.g. for fee calculations) but don't have the requisite signing key yet and will fill out the
        ///  real signature later.
        /// </summary>
        /// <returns>
        /// The <see cref="TransactionSignature"/>.
        /// </returns>
        public static TransactionSignature Dummy()
        {
            BigInteger val = EcKey.HalfCurveOrder;
            return new TransactionSignature(val, val);
        }

        public static TransactionSignature DecodeFromBitcoin(byte[] bytes, bool requireCanonicalEncoding, bool requireCanonicalSValue)
        {
            // Bitcoin encoding is DER signature + sighash byte.
            if (requireCanonicalEncoding && !IsEncodingCanonical(bytes))
            {
                throw new TransactionException("Signature encoding is not canonical.");
            }

            EcKey.EcdsaSignature sig;
            try
            {
                sig = DecodeFromDer(bytes);
            }
            catch (Exception e)
            {
                throw new TransactionException("Could not decode DER", e);
            }

            if (requireCanonicalSValue && !sig.IsCanonical())
            {
                throw new TransactionException("S-value is not canonical.");
            }

            // In Bitcoin, any value of the final byte is valid, but not necessarily canonical. must store the exact byte found.
            return new TransactionSignature(sig.R, sig.S, (Transaction.SigHash)bytes.Last());
        }

        /// <summary>
        /// Returns true if the given signature is has canonical encoding, and will thus be accepted as standard by
        /// Bitcoin Core. DER and the SIGHASH encoding allow for quite some flexibility in how the same structures
        /// are encoded, and this can open up novel attacks in which a man in the middle takes a transaction and then
        /// changes its signature such that the transaction hash is different but it's still valid. This can confuse wallets
        /// and generally violates people's mental model of how Bitcoin should work, thus, non-canonical signatures are now
        /// not relayed by default.
        /// </summary>
        public static bool IsEncodingCanonical(byte[] signature)
        {
            // See Bitcoin Core's IsCanonicalSignature, https://bitcointalk.org/index.php?topic=8392.msg127623#msg127623
            // A canonical signature exists of: <30> <total len> <02> <len R> <R> <02> <len S> <S> <hashtype>
            // Where R and S are not negative (their first byte has its highest bit not set), and not
            // excessively padded (do not start with a 0 byte, unless an otherwise negative number follows,
            // in which case a single 0 byte is necessary and even required).
            if (signature.Length < 9 || signature.Length > 73)
            {
                return false;
            }

            int hashType = signature[signature.Length - 1] & ~Transaction.SighashAnyonecanpayValue;
            if (hashType < ((int)Transaction.SigHash.All) || hashType > ((int)Transaction.SigHash.Single))
            {
                return false;
            }

            // "wrong type"                  "wrong length marker"
            if ((signature[0] & 0xff) != 0x30 || (signature[1] & 0xff) != signature.Length - 3)
            {
                return false;
            }

            int lenR = signature[3] & 0xff;
            if (5 + lenR >= signature.Length || lenR == 0)
            {
                return false;
            }

            int lenS = signature[5 + lenR] & 0xff;
            if (lenR + lenS + 7 != signature.Length || lenS == 0)
            {
                return false;
            }

            // R value type mismatch          R value negative
            if (signature[4 - 2] != 0x02 || (signature[4] & 0x80) == 0x80)
            {
                return false;
            }

            if (lenR > 1 && signature[4] == 0x00 && (signature[4 + 1] & 0x80) != 0x80)
            {
                return false; // R value excessively padded
            }

            // S value type mismatch                    S value negative
            if (signature[6 + lenR - 2] != 0x02 || (signature[6 + lenR] & 0x80) == 0x80)
            {
                return false;
            }

            if (lenS > 1 && signature[6 + lenR] == 0x00 && (signature[6 + lenR + 1] & 0x80) != 0x80)
            {
                return false; // S value excessively padded
            }

            return true;
        }

        /// <summary>
        /// What we get back from the signer are the two components of a signature, r and s. To get a flat byte stream
        ///  of the type used by Bitcoin we have to encode them using DER encoding, which is just a way to pack the two
        ///  components into a structure, and then we append a byte to the end for the sighash flags.
        /// </summary>
        public byte[] EncodeToBitcoin()
        {
            return this.EncodeToDer((byte)this.sighash);
        }

        /// <summary>
        /// The to canonical form.
        /// </summary>
        /// <returns>
        /// The <see cref="EcKey.EcdsaSignature"/>.
        /// </returns>
        public override EcKey.EcdsaSignature ToCanonicalised()
        {
            return new TransactionSignature(base.ToCanonicalised(), this.sighash);
        }
    }
}
