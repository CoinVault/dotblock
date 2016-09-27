// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestAutoLanguageDetect.cs" company="Dark Caesium">
//   Copyright (c) Dark Caesium.  All rights reserved.
//   //   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Blockchain.Protocol.Bitcoin.Test
{
    #region Using Directives

    using Blockchain.Protocol.Bitcoin.Mnemonic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    /// <summary>
    /// The test auto language detect.
    /// </summary>
    [TestClass]
    public class TestAutoLanguageDetect
    {
        /// <summary>
        /// The test known english.
        /// </summary>
        [TestMethod]
        public void TestKnownEnglish()
        {
            Assert.AreEqual(Bip39.Language.English, Bip39.AutoDetectLanguageOfWords(new[] { "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "about" }));
        }

        /// <summary>
        /// The test known japenese.
        /// </summary>
        [TestMethod]
        public void TestKnownJapenese()
        {
            Assert.AreEqual(Bip39.Language.Japanese, Bip39.AutoDetectLanguageOfWords(new[] { "あいこくしん", "あいさつ", "あいだ", "あおぞら", "あかちゃん", "あきる", "あけがた", "あける", "あこがれる", "あさい", "あさひ", "あしあと", "あじわう", "あずかる", "あずき", "あそぶ", "あたえる", "あたためる", "あたりまえ", "あたる", "あつい", "あつかう", "あっしゅく", "あつまり", "あつめる", "あてな", "あてはまる", "あひる", "あぶら", "あぶる", "あふれる", "あまい", "あまど", "あまやかす", "あまり", "あみもの", "あめりか" }));
        }

        /// <summary>
        /// The test known spanish.
        /// </summary>
        [TestMethod]
        public void TestKnownSpanish()
        {
            Assert.AreEqual(Bip39.Language.Spanish, Bip39.AutoDetectLanguageOfWords(new[] { "yoga", "yogur", "zafiro", "zanja", "zapato", "zarza", "zona", "zorro", "zumo", "zurdo" }));
        }

        /// <summary>
        /// The test known chinese simplified.
        /// </summary>
        [TestMethod]
        public void TestKnownChineseSimplified()
        {
            Assert.AreEqual(Bip39.Language.ChineseSimplified, Bip39.AutoDetectLanguageOfWords(new[] { "的", "一", "是", "在", "不", "了", "有", "和", "人", "这" }));
        }

        /// <summary>
        /// The test known chinese traditional.
        /// </summary>
        [TestMethod]
        public void TestKnownChineseTraditional()
        {
            Assert.AreEqual(Bip39.Language.ChineseTraditional, Bip39.AutoDetectLanguageOfWords(new[] { "的", "一", "是", "在", "不", "了", "有", "和", "載" }));
        }

        /// <summary>
        /// The test known french.
        /// </summary>
        [TestMethod]
        public void TestKnownFrench()
        {
            Assert.AreEqual(Bip39.Language.French, Bip39.AutoDetectLanguageOfWords(new[] { "abaisser", "brutal", "bulletin", "circuler", "citoyen", "impact", "joyeux", "massif", "nébuleux" }));
        }

        /// <summary>
        /// The test known unknown.
        /// </summary>
        [TestMethod]
        public void TestKnownUnknown()
        {
            Assert.AreEqual(Bip39.Language.Unknown, Bip39.AutoDetectLanguageOfWords(new[] { "gffgfg", "khjkjk", "kjkkj" }));
        }
    }
}
