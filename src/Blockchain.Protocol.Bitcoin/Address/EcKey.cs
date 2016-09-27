// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EcKey.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Blockchain.Protocol.Bitcoin.Address
{
    #region Using Directives

    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Blockchain.Protocol.Bitcoin.Security.Cryptography;

    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Asn1.Sec;
    using Org.BouncyCastle.Asn1.X9;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Crypto.Signers;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Security;

    #endregion

    /// <summary>
    /// Represents an elliptic curve key pair that we own and can use for signing transactions. Currently,
    /// Bouncy Castle is used. In future this may become an interface with multiple implementations using different crypto
    /// libraries. The class also provides a static method that can verify a signature with just the public key.
    /// </summary>
    public class EcKey
    {
        public static BigInteger HalfCurveOrder;

        public static BigInteger CurveOrder;

        public static readonly ECDomainParameters EcParams;

        public static readonly X9ECParameters X9EcParameters;

        private readonly BigInteger privateKey;

        private readonly byte[] publicKey;

        /// <summary>
        /// Initializes static members of the <see cref="EcKey"/> class.
        /// </summary>
        static EcKey()
        {
            // All clients must agree on the curve to use by agreement. BitCoin uses secp256k1.
            X9EcParameters = SecNamedCurves.GetByName("secp256k1");
            EcParams = new ECDomainParameters(X9EcParameters.Curve, X9EcParameters.G, X9EcParameters.N, X9EcParameters.H);
            HalfCurveOrder = X9EcParameters.N.ShiftRight(1);
            CurveOrder = X9EcParameters.N;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcKey"/> class. 
        /// Creates an ECKey given only the private key. This works because EC public keys are derivable from their
        /// private keys by doing a multiply with the generator value.
        /// </summary>
        /// <param name="privKey">
        /// The private Key.
        /// </param>
        /// <param name="compressed">
        /// The compressed.
        /// </param>
        public EcKey(BigInteger privKey, bool compressed = true)
        {
            this.privateKey = privKey;
            this.publicKey = PublicKeyFromPrivate(privKey, compressed);
            this.Compressed = compressed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the public key should be compressed.
        /// </summary>
        public bool Compressed { get; set; }

        /// <summary>
        /// Gets the raw public key value. This appears in transaction scriptSigs. Note that this is <b>not</b> the same
        /// as the pubKeyHash/address.
        /// </summary>
        public byte[] PublicKeyBytes
        {
            get
            {
                return this.publicKey;
            }
        }

        /// <summary>
        /// Verifies the given ECDSA signature against the message bytes using the public key bytes.
        /// </summary>
        public static bool Verify(byte[] data, EcdsaSignature signature, byte[] pubkey)
        {
            var signer = new ECDsaSigner();
            var q = X9EcParameters.Curve.DecodePoint(pubkey);
            var ecp = new ECPublicKeyParameters("EC", q, EcParams);
            signer.Init(false, ecp);
            return signer.VerifySignature(data, signature.R, signature.S);
        }

        /// <summary>
        /// Verifies the given ECDSA signature against the message bytes using the public key bytes.
        /// </summary>
        public bool Verify(byte[] data, EcdsaSignature signature)
        {
            return Verify(data, signature, this.PublicKeyBytes);
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append("PublicKey:").Append(CryptoUtil.ToHex(this.publicKey));
            b.Append(" PrivateKey:").Append(CryptoUtil.ToHex(this.privateKey.ToByteArray()));
            return b.ToString();
        }

        /// <summary>
        /// Returns the public key representation of this EC public key bytes taking compression in to account. 
        /// </summary>
        public BitcoinPublicKey ToPublicKey()
        {
            return new BitcoinPublicKey(this.PublicKeyBytes, this.Compressed);
        }

        /// <summary>
        /// Calculates an ECDSA signature in DER format for the given input hash. Note that the input is expected to be
        /// 32 bytes long.
        /// </summary>
        public EcdsaSignature Sign(byte[] input)
        {
            ECDsaSigner signer = new ECDsaSigner(new HMacDsaKCalculator(DigestUtilities.GetDigest("SHA-256")));
            ////ECDsaSigner signer = new ECDsaSigner();
            signer.Init(true, new ECPrivateKeyParameters(this.privateKey, EcParams));
            BigInteger[] sig = signer.GenerateSignature(input);
            return new EcdsaSignature(sig[0], sig[1]).ToCanonicalised();
        }


        /// <summary>
        /// Returns a 32 byte array containing the private key.
        /// </summary>
        public byte[] GetPrivKeyBytes()
        {
            //// Getting the bytes out of a BigInteger gives us an extra zero byte on the end (for signedness)
            //// or less than 32 bytes (leading zeros).  Coerce to 32 bytes in all cases.
            var bytes = new byte[32];

            var privArray = this.privateKey.ToByteArray();
            var privStart = (privArray.Length == 33) ? 1 : 0;
            var privLength = Math.Min(privArray.Length, 32);
            Array.Copy(privArray, privStart, bytes, 32 - privLength, privLength);

            return bytes;
        }

        /// <summary>
        /// Returns the private key representation of this EC private key bytes. 
        /// </summary>
        public BitcoinPrivateKey GetPrivateKeyEncoded()
        {
            return new BitcoinPrivateKey(this.GetPrivKeyBytes());
        }

        /// <summary>
        /// Derive the public key by doing a point multiply of G * private key.
        /// </summary>
        /// <param name="privKey">
        /// The private Key.
        /// </param>
        /// <param name="compressed">
        /// Create a compressed pub key.
        /// </param>
        private static byte[] PublicKeyFromPrivate(BigInteger privKey, bool compressed = true)
        {
            var pparam = new ECPrivateKeyParameters(privKey, EcParams);
            var point = X9EcParameters.G.Multiply(pparam.D);
            var ecp = new ECPublicKeyParameters("EC", point, EcParams);

            return EcParams.Curve.CreatePoint(ecp.Q.AffineXCoord.ToBigInteger(), ecp.Q.AffineYCoord.ToBigInteger()).GetEncoded(compressed);
        }

        /// <summary>
        /// Groups the two components that make up a signature, and provides a way to encode to DER form, which is
        /// how ECDSA signatures are represented when embedded in other data structures in the Bitcoin protocol. The raw
        /// components can be useful for doing further EC maths on them.
        /// </summary>
        public class EcdsaSignature
        {
            /// <summary>
            /// Gets or sets the r.
            /// </summary>
            public BigInteger R { get; set; }

            /// <summary>
            /// Gets or sets the s.
            /// </summary>
            public BigInteger S { get; set; }


            /// <summary>
            /// Initializes a new instance of the <see cref="EcdsaSignature"/> class. 
            /// Constructs a signature with the given components. Does NOT automatically canonicalise the signature.
            /// </summary>
            /// <param name="r">
            /// The R value of the signature.
            /// </param>
            /// <param name="s">
            /// The S value of the signature.
            /// </param>
            public EcdsaSignature(BigInteger r, BigInteger s)
            {
                this.R = r;
                this.S = s;
            }

            /// <summary>
            /// Returns true if the S component is "low", that means it is below {@link ECKey#HALF_CURVE_ORDER}. 
            /// See <a href="https://github.com/bitcoin/bips/blob/master/bip-0062.mediawiki#Low_S_values_in_signatures">BIP62</a>.
            /// </summary>
            public bool IsCanonical()
            {
                return this.S.CompareTo(HalfCurveOrder) <= 0;
            }

            /// <summary>
            /// Will automatically adjust the S component to be less than or equal to half the curve order, if necessary.
            /// This is required because for every signature (r,s) the signature (r, -s (mod N)) is a valid signature of
            /// the same message. However, we dislike the ability to modify the bits of a Bitcoin transaction after it's
            /// been signed, as that violates various assumed invariants. Thus in future only one of those forms will be
            /// considered legal and the other will be banned.
            /// </summary>
            public virtual EcdsaSignature ToCanonicalised()
            {
                if (!this.IsCanonical())
                {
                    // The order of the curve is the number of valid points that exist on that curve. If S is in the upper
                    // half of the number of valid points, then bring it back to the lower half. Otherwise, imagine that
                    // N = 10
                    // s = 8, so (-8 % 10 == 2) thus both (r, 8) and (r, 2) are valid solutions.
                    // 10 - 8 == 2, giving us always the latter solution, which is canonical.
                    return new EcdsaSignature(this.R, EcParams.N.Subtract(this.S));
                }

                return this;
            }

            /// <summary>
            /// DER is an international standard for serializing data structures which is widely used in cryptography.
            /// It's somewhat like protocol buffers but less convenient. This method returns a standard DER encoding
            /// of the signature, as recognized by OpenSSL and other libraries.
            /// </summary>
            /// <param name="sighash">
            /// The signature hash.
            /// </param>
            public byte[] EncodeToDer(byte sighash)
            {
                // Usually 70-72 bytes.
                MemoryStream bos = new MemoryStream(72);
                DerSequenceGenerator seq = new DerSequenceGenerator(bos);
                seq.AddObject(new DerInteger(this.R));
                seq.AddObject(new DerInteger(this.S));
                seq.Close();
                return bos.ToArray().Concat(new[] { sighash }).ToArray();
            }

            /// <summary>
            /// Decode From DER
            /// </summary>
            public static EcdsaSignature DecodeFromDer(byte[] bytes)
            {
                try
                {
                    Asn1InputStream decoder = new Asn1InputStream(bytes);

                    var seq = decoder.ReadObject() as DerSequence;
                    if (seq == null || seq.Count != 2)
                    {
                        throw new FormatException("InvalidDERSignature");
                    }

                    return new EcdsaSignature(((DerInteger)seq[0]).Value, ((DerInteger)seq[1]).Value);
                }
                catch (IOException ex)
                {
                    throw new FormatException("InvalidDERSignature", ex);
                }
            }

            public override bool Equals(object o)
            {
                if (this == o)
                {
                    return true;
                }

                if (o == null)
                {
                    return false;
                }

                EcdsaSignature other = (EcdsaSignature)o;
                return this.R.Equals(other.R) && this.S.Equals(other.S);
            }

            public override int GetHashCode()
            {
                return this.R.GetHashCode() + this.S.GetHashCode();
            }
        }
    }
}