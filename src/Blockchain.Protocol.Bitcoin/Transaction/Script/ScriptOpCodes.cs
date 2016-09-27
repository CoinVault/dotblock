// <copyright file="ScriptOpCodes.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#pragma warning disable 1591
namespace Blockchain.Protocol.Bitcoin.Transaction.Script
{
    using System.Collections.Generic;

    /// <summary>
    /// The script op codes.
    /// Borrowed from bitcoinj implementation
    /// </summary>
    public class ScriptOpCodes
    {

        // push value
        public const int OP_0 = 0x00; // push empty vector
        public static int OP_FALSE = OP_0;
        public const int OP_PUSHDATA1 = 0x4c;
        public const int OP_PUSHDATA2 = 0x4d;
        public const int OP_PUSHDATA4 = 0x4e;
        public const int OP_1NEGATE = 0x4f;
        public const int OP_RESERVED = 0x50;
        public const int OP_1 = 0x51;
        public const int OP_TRUE = OP_1;
        public const int OP_2 = 0x52;
        public const int OP_3 = 0x53;
        public const int OP_4 = 0x54;
        public const int OP_5 = 0x55;
        public const int OP_6 = 0x56;
        public const int OP_7 = 0x57;
        public const int OP_8 = 0x58;
        public const int OP_9 = 0x59;
        public const int OP_10 = 0x5a;
        public const int OP_11 = 0x5b;
        public const int OP_12 = 0x5c;
        public const int OP_13 = 0x5d;
        public const int OP_14 = 0x5e;
        public const int OP_15 = 0x5f;
        public const int OP_16 = 0x60;

        // control
        public const int OP_NOP = 0x61;
        public const int OP_VER = 0x62;
        public const int OP_IF = 0x63;
        public const int OP_NOTIF = 0x64;
        public const int OP_VERIF = 0x65;
        public const int OP_VERNOTIF = 0x66;
        public const int OP_ELSE = 0x67;
        public const int OP_ENDIF = 0x68;
        public const int OP_VERIFY = 0x69;
        public const int OP_RETURN = 0x6a;

        // stack ops
        public const int OP_TOALTSTACK = 0x6b;
        public const int OP_FROMALTSTACK = 0x6c;
        public const int OP_2DROP = 0x6d;
        public const int OP_2DUP = 0x6e;
        public const int OP_3DUP = 0x6f;
        public const int OP_2OVER = 0x70;
        public const int OP_2ROT = 0x71;
        public const int OP_2SWAP = 0x72;
        public const int OP_IFDUP = 0x73;
        public const int OP_DEPTH = 0x74;
        public const int OP_DROP = 0x75;
        public const int OP_DUP = 0x76;
        public const int OP_NIP = 0x77;
        public const int OP_OVER = 0x78;
        public const int OP_PICK = 0x79;
        public const int OP_ROLL = 0x7a;
        public const int OP_ROT = 0x7b;
        public const int OP_SWAP = 0x7c;
        public const int OP_TUCK = 0x7d;

        // splice ops
        public const int OP_CAT = 0x7e;
        public const int OP_SUBSTR = 0x7f;
        public const int OP_LEFT = 0x80;
        public const int OP_RIGHT = 0x81;
        public const int OP_SIZE = 0x82;

        // bit logic
        public const int OP_INVERT = 0x83;
        public const int OP_AND = 0x84;
        public const int OP_OR = 0x85;
        public const int OP_XOR = 0x86;
        public const int OP_EQUAL = 0x87;
        public const int OP_EQUALVERIFY = 0x88;
        public const int OP_RESERVED1 = 0x89;
        public const int OP_RESERVED2 = 0x8a;

        // numeric
        public const int OP_1ADD = 0x8b;
        public const int OP_1SUB = 0x8c;
        public const int OP_2MUL = 0x8d;
        public const int OP_2DIV = 0x8e;
        public const int OP_NEGATE = 0x8f;
        public const int OP_ABS = 0x90;
        public const int OP_NOT = 0x91;
        public const int OP_0NOTEQUAL = 0x92;
        public const int OP_ADD = 0x93;
        public const int OP_SUB = 0x94;
        public const int OP_MUL = 0x95;
        public const int OP_DIV = 0x96;
        public const int OP_MOD = 0x97;
        public const int OP_LSHIFT = 0x98;
        public const int OP_RSHIFT = 0x99;
        public const int OP_BOOLAND = 0x9a;
        public const int OP_BOOLOR = 0x9b;
        public const int OP_NUMEQUAL = 0x9c;
        public const int OP_NUMEQUALVERIFY = 0x9d;
        public const int OP_NUMNOTEQUAL = 0x9e;
        public const int OP_LESSTHAN = 0x9f;
        public const int OP_GREATERTHAN = 0xa0;
        public const int OP_LESSTHANOREQUAL = 0xa1;
        public const int OP_GREATERTHANOREQUAL = 0xa2;
        public const int OP_MIN = 0xa3;
        public const int OP_MAX = 0xa4;
        public const int OP_WITHIN = 0xa5;

        // crypto
        public const int OP_RIPEMD160 = 0xa6;
        public const int OP_SHA1 = 0xa7;
        public const int OP_SHA256 = 0xa8;
        public const int OP_HASH160 = 0xa9;
        public const int OP_HASH256 = 0xaa;
        public const int OP_CODESEPARATOR = 0xab;
        public const int OP_CHECKSIG = 0xac;
        public const int OP_CHECKSIGVERIFY = 0xad;
        public const int OP_CHECKMULTISIG = 0xae;
        public const int OP_CHECKMULTISIGVERIFY = 0xaf;

        // block state
        /** Check lock time of the block. Introduced in BIP 65, replacing OP_NOP2 */
        public const int OP_CHECKLOCKTIMEVERIFY = 0xb1;

        // expansion
        public const int OP_NOP1 = 0xb0;
        /** Deprecated by BIP 65 */
        #region Deprecated
    
            public const int OP_NOP2 = OP_CHECKLOCKTIMEVERIFY;
        public const int OP_NOP3 = 0xb2;
        public const int OP_NOP4 = 0xb3;
        public const int OP_NOP5 = 0xb4;
        public const int OP_NOP6 = 0xb5;
        public const int OP_NOP7 = 0xb6;
        public const int OP_NOP8 = 0xb7;
        public const int OP_NOP9 = 0xb8;
        public const int OP_NOP10 = 0xb9;
        public const int OP_INVALIDOPCODE = 0xff;
  
        #endregion 

        private static readonly Dictionary<int, string> opCodeMap = new Dictionary<int, string> {
            {OP_0, "0"},
            {OP_PUSHDATA1, "PUSHDATA1"},
            {OP_PUSHDATA2, "PUSHDATA2"},
            {OP_PUSHDATA4, "PUSHDATA4"},
            {OP_1NEGATE, "1NEGATE"},
            {OP_RESERVED, "RESERVED"},
            {OP_1, "1"},
            {OP_2, "2"},
            {OP_3, "3"},
            {OP_4, "4"},
            {OP_5, "5"},
            {OP_6, "6"},
            {OP_7, "7"},
            {OP_8, "8"},
            {OP_9, "9"},
            {OP_10, "10"},
            {OP_11, "11"},
            {OP_12, "12"},
            {OP_13, "13"},
            {OP_14, "14"},
            {OP_15, "15"},
            {OP_16, "16"},
            {OP_NOP, "NOP"},
            {OP_VER, "VER"},
            {OP_IF, "IF"},
            {OP_NOTIF, "NOTIF"},
            {OP_VERIF, "VERIF"},
            {OP_VERNOTIF, "VERNOTIF"},
            {OP_ELSE, "ELSE"},
            {OP_ENDIF, "ENDIF"},
            {OP_VERIFY, "VERIFY"},
            {OP_RETURN, "RETURN"},
            {OP_TOALTSTACK, "TOALTSTACK"},
            {OP_FROMALTSTACK, "FROMALTSTACK"},
            {OP_2DROP, "2DROP"},
            {OP_2DUP, "2DUP"},
            {OP_3DUP, "3DUP"},
            {OP_2OVER, "2OVER"},
            {OP_2ROT, "2ROT"},
            {OP_2SWAP, "2SWAP"},
            {OP_IFDUP, "IFDUP"},
            {OP_DEPTH, "DEPTH"},
            {OP_DROP, "DROP"},
            {OP_DUP, "DUP"},
            {OP_NIP, "NIP"},
            {OP_OVER, "OVER"},
            {OP_PICK, "PICK"},
            {OP_ROLL, "ROLL"},
            {OP_ROT, "ROT"},
            {OP_SWAP, "SWAP"},
            {OP_TUCK, "TUCK"},
            {OP_CAT, "CAT"},
            {OP_SUBSTR, "SUBSTR"},
            {OP_LEFT, "LEFT"},
            {OP_RIGHT, "RIGHT"},
            {OP_SIZE, "SIZE"},
            {OP_INVERT, "INVERT"},
            {OP_AND, "AND"},
            {OP_OR, "OR"},
            {OP_XOR, "XOR"},
            {OP_EQUAL, "EQUAL"},
            {OP_EQUALVERIFY, "EQUALVERIFY"},
            {OP_RESERVED1, "RESERVED1"},
            {OP_RESERVED2, "RESERVED2"},
            {OP_1ADD, "1ADD"},
            {OP_1SUB, "1SUB"},
            {OP_2MUL, "2MUL"},
            {OP_2DIV, "2DIV"},
            {OP_NEGATE, "NEGATE"},
            {OP_ABS, "ABS"},
            {OP_NOT, "NOT"},
            {OP_0NOTEQUAL, "0NOTEQUAL"},
            {OP_ADD, "ADD"},
            {OP_SUB, "SUB"},
            {OP_MUL, "MUL"},
            {OP_DIV, "DIV"},
            {OP_MOD, "MOD"},
            {OP_LSHIFT, "LSHIFT"},
            {OP_RSHIFT, "RSHIFT"},
            {OP_BOOLAND, "BOOLAND"},
            {OP_BOOLOR, "BOOLOR"},
            {OP_NUMEQUAL, "NUMEQUAL"},
            {OP_NUMEQUALVERIFY, "NUMEQUALVERIFY"},
            {OP_NUMNOTEQUAL, "NUMNOTEQUAL"},
            {OP_LESSTHAN, "LESSTHAN"},
            {OP_GREATERTHAN, "GREATERTHAN"},
            {OP_LESSTHANOREQUAL, "LESSTHANOREQUAL"},
            {OP_GREATERTHANOREQUAL, "GREATERTHANOREQUAL"},
            {OP_MIN, "MIN"},
            {OP_MAX, "MAX"},
            {OP_WITHIN, "WITHIN"},
            {OP_RIPEMD160, "RIPEMD160"},
            {OP_SHA1, "SHA1"},
            {OP_SHA256, "SHA256"},
            {OP_HASH160, "HASH160"},
            {OP_HASH256, "HASH256"},
            {OP_CODESEPARATOR, "CODESEPARATOR"},
            {OP_CHECKSIG, "CHECKSIG"},
            {OP_CHECKSIGVERIFY, "CHECKSIGVERIFY"},
            {OP_CHECKMULTISIG, "CHECKMULTISIG"},
            {OP_CHECKMULTISIGVERIFY, "CHECKMULTISIGVERIFY"},
            {OP_NOP1, "NOP1"},
            {OP_CHECKLOCKTIMEVERIFY, "CHECKLOCKTIMEVERIFY"},
            {OP_NOP3, "NOP3"},
            {OP_NOP4, "NOP4"},
            {OP_NOP5, "NOP5"},
            {OP_NOP6, "NOP6"},
            {OP_NOP7, "NOP7"},
            {OP_NOP8, "NOP8"},
            {OP_NOP9, "NOP9"},
            {OP_NOP10, "NOP10"}};

        private static readonly Dictionary<string, int> opCodeNameMap = new Dictionary<string, int>{
            {"0", OP_0},
            {"PUSHDATA1", OP_PUSHDATA1},
            {"PUSHDATA2", OP_PUSHDATA2},
            {"PUSHDATA4", OP_PUSHDATA4},
            {"1NEGATE", OP_1NEGATE},
            {"RESERVED", OP_RESERVED},
            {"1", OP_1},
            {"2", OP_2},
            {"3", OP_3},
            {"4", OP_4},
            {"5", OP_5},
            {"6", OP_6},
            {"7", OP_7},
            {"8", OP_8},
            {"9", OP_9},
            {"10", OP_10},
            {"11", OP_11},
            {"12", OP_12},
            {"13", OP_13},
            {"14", OP_14},
            {"15", OP_15},
            {"16", OP_16},
            {"NOP", OP_NOP},
            {"VER", OP_VER},
            {"IF", OP_IF},
            {"NOTIF", OP_NOTIF},
            {"VERIF", OP_VERIF},
            {"VERNOTIF", OP_VERNOTIF},
            {"ELSE", OP_ELSE},
            {"ENDIF", OP_ENDIF},
            {"VERIFY", OP_VERIFY},
            {"RETURN", OP_RETURN},
            {"TOALTSTACK", OP_TOALTSTACK},
            {"FROMALTSTACK", OP_FROMALTSTACK},
            {"2DROP", OP_2DROP},
            {"2DUP", OP_2DUP},
            {"3DUP", OP_3DUP},
            {"2OVER", OP_2OVER},
            {"2ROT", OP_2ROT},
            {"2SWAP", OP_2SWAP},
            {"IFDUP", OP_IFDUP},
            {"DEPTH", OP_DEPTH},
            {"DROP", OP_DROP},
            {"DUP", OP_DUP},
            {"NIP", OP_NIP},
            {"OVER", OP_OVER},
            {"PICK", OP_PICK},
            {"ROLL", OP_ROLL},
            {"ROT", OP_ROT},
            {"SWAP", OP_SWAP},
            {"TUCK", OP_TUCK},
            {"CAT", OP_CAT},
            {"SUBSTR", OP_SUBSTR},
            {"LEFT", OP_LEFT},
            {"RIGHT", OP_RIGHT},
            {"SIZE", OP_SIZE},
            {"INVERT", OP_INVERT},
            {"AND", OP_AND},
            {"OR", OP_OR},
            {"XOR", OP_XOR},
            {"EQUAL", OP_EQUAL},
            {"EQUALVERIFY", OP_EQUALVERIFY},
            {"RESERVED1", OP_RESERVED1},
            {"RESERVED2", OP_RESERVED2},
            {"1ADD", OP_1ADD},
            {"1SUB", OP_1SUB},
            {"2MUL", OP_2MUL},
            {"2DIV", OP_2DIV},
            {"NEGATE", OP_NEGATE},
            {"ABS", OP_ABS},
            {"NOT", OP_NOT},
            {"0NOTEQUAL", OP_0NOTEQUAL},
            {"ADD", OP_ADD},
            {"SUB", OP_SUB},
            {"MUL", OP_MUL},
            {"DIV", OP_DIV},
            {"MOD", OP_MOD},
            {"LSHIFT", OP_LSHIFT},
            {"RSHIFT", OP_RSHIFT},
            {"BOOLAND", OP_BOOLAND},
            {"BOOLOR", OP_BOOLOR},
            {"NUMEQUAL", OP_NUMEQUAL},
            {"NUMEQUALVERIFY", OP_NUMEQUALVERIFY},
            {"NUMNOTEQUAL", OP_NUMNOTEQUAL},
            {"LESSTHAN", OP_LESSTHAN},
            {"GREATERTHAN", OP_GREATERTHAN},
            {"LESSTHANOREQUAL", OP_LESSTHANOREQUAL},
            {"GREATERTHANOREQUAL", OP_GREATERTHANOREQUAL},
            {"MIN", OP_MIN},
            {"MAX", OP_MAX},
            {"WITHIN", OP_WITHIN},
            {"RIPEMD160", OP_RIPEMD160},
            {"SHA1", OP_SHA1},
            {"SHA256", OP_SHA256},
            {"HASH160", OP_HASH160},
            {"HASH256", OP_HASH256},
            {"CODESEPARATOR", OP_CODESEPARATOR},
            {"CHECKSIG", OP_CHECKSIG},
            {"CHECKSIGVERIFY", OP_CHECKSIGVERIFY},
            {"CHECKMULTISIG", OP_CHECKMULTISIG},
            {"CHECKMULTISIGVERIFY", OP_CHECKMULTISIGVERIFY},
            {"NOP1", OP_NOP1},
            {"CHECKLOCKTIMEVERIFY", OP_CHECKLOCKTIMEVERIFY},
            {"NOP2", OP_NOP2},
            {"NOP3", OP_NOP3},
            {"NOP4", OP_NOP4},
            {"NOP5", OP_NOP5},
            {"NOP6", OP_NOP6},
            {"NOP7", OP_NOP7},
            {"NOP8", OP_NOP8},
            {"NOP9", OP_NOP9},
            {"NOP10", OP_NOP10}};

        /// <summary>
        /// Converts the given OpCode into a string (eg "0", "PUSHDATA", or "NON_OP(10)").
        /// </summary>
        public static string GetOpCodeName(int opcode)
        {
            string ret;
            if (opCodeMap.TryGetValue(opcode, out ret))
            {
                return ret;
            }

            return "NON_OP(" + opcode + ")";
        }

        /// <summary>
        /// Converts the given pushdata OpCode into a string (eg "PUSHDATA2", or "PUSHDATA(23)")
        /// </summary>
        public static string GetPushDataName(int opcode)
        {
            string ret;
            if (opCodeMap.TryGetValue(opcode, out ret))
            {
                return ret;
            }
            return "PUSHDATA(" + opcode + ")";
        }

        /// <summary>
        /// Converts the given OpCodeName into an int
        /// </summary>
        public static int GetOpCode(string opCodeName)
        {
            int ret;
            if (opCodeNameMap.TryGetValue(opCodeName, out ret))
            {
                return ret;
            }
            return OP_INVALIDOPCODE;
        }
    }
}

