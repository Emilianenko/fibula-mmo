﻿// -----------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Common.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Static class that contains helper extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// The double quote character.
        /// </summary>
        private const char DoubleQuote = '"';

        /// <summary>
        /// The back slash character.
        /// </summary>
        private const char Backslash = '\\';

        /// <summary>
        /// The space character.
        /// </summary>
        private const char Space = ' ';

        /// <summary>
        /// Retrieves the enclosed strings within a string.
        /// </summary>
        /// <param name="str">The string from which to retrieve the enclosured strings.</param>
        /// <param name="openClosePairs">A collection of open and close characters between which to look for enclosed strings.</param>
        /// <returns>A collection of enclosed string contained in the string.</returns>
        public static IEnumerable<string> GetEnclosedStrings(this string str, IEnumerable<(char openChar, char closeChar)> openClosePairs)
        {
            var stack = new Stack<string>();

            if (string.IsNullOrWhiteSpace(str))
            {
                return stack;
            }

            var inQuote = false;
            var openEnclosure = new Stack<char>();
            var buffers = new Stack<StringBuilder>();

            var main = new StringBuilder();
            buffers.Push(main);

            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == DoubleQuote && (i > 0 && str[i - 1] != Backslash))
                {
                    inQuote = !inQuote;
                }

                var c = str[i];

                if (inQuote || !openClosePairs.Any(t => t.openChar == c || t.closeChar == c))
                {
                    buffers.Peek().Append(c);
                    continue;
                }

                var (openChar, closeChar) = openClosePairs.Single(t => t.openChar == c || t.closeChar == c);

                if (openChar == c)
                {
                    openEnclosure.Push(c);

                    buffers.Push(new StringBuilder());

                    continue;
                }

                if (closeChar == c)
                {
                    if (openEnclosure.Peek() != openChar)
                    {
                        throw new InvalidOperationException("Malformed string.");
                    }

                    openEnclosure.Pop();
                    stack.Push(buffers.Pop().ToString());
                }
            }

            while (buffers.Count > 0)
            {
                stack.Push(buffers.Pop().ToString());
            }

            return stack;
        }

        /// <summary>
        /// Trims the open and close chars from the string.
        /// </summary>
        /// <param name="inputStr">The string to trim the pairs from.</param>
        /// <param name="openClosePairs">The collection of open and close pairs.</param>
        /// <returns>The resulting string after trim.</returns>
        public static string TrimEnclosures(this string inputStr, IEnumerable<(char openChar, char closeChar)> openClosePairs)
        {
            foreach (var (openChar, closeChar) in openClosePairs)
            {
                if (inputStr.StartsWith(openChar.ToString()) && inputStr.EndsWith(closeChar.ToString()))
                {
                    inputStr = inputStr[1..];
                    inputStr = inputStr[0..^1];
                }
            }

            return inputStr;
        }

        /// <summary>
        /// Splits a string using the given token, but preserving any quoted strings.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="token">Optional. The token to split by. Defaults to <see cref="Space"/>.</param>
        /// <returns>A collection of strings resulting from the split.</returns>
        public static IEnumerable<string> SplitByToken(this string str, char token = Space)
        {
            var stringsFound = new List<string>();

            if (string.IsNullOrWhiteSpace(str))
            {
                return stringsFound;
            }

            var inQuote = false;
            var sb = new StringBuilder();

            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == DoubleQuote && (i > 0 && str[i - 1] != Backslash))
                {
                    inQuote = !inQuote;
                }

                if (str[i] == token && !inQuote)
                {
                    var currentString = sb.ToString();

                    if (!string.IsNullOrWhiteSpace(currentString))
                    {
                        stringsFound.Add(currentString);
                        sb.Clear();
                    }
                }
                else
                {
                    sb.Append(str[i]);
                }
            }

            if (!string.IsNullOrWhiteSpace(sb.ToString()))
            {
                stringsFound.Add(sb.ToString());
            }

            return stringsFound;
        }

        /// <summary>
        /// Converts a char to a byte.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte ToByte(this char value)
        {
            return (byte)value;
        }

        /// <summary>
        /// Converts a string to a byte array.
        /// </summary>
        /// <returns></returns>
        public static byte[] ToByteArray(this string s)
        {
            var value = new List<byte>();
            foreach (var c in s)
            {
                value.Add(c.ToByte());
            }

            return value.ToArray();
        }

        /// <summary>Convert a string of hex digits (ex: E4 CA B2) to a byte array.</summary>
        /// <param name="s">The string containing the hex digits (with or without spaces).</param>
        /// <returns>Returns an array of bytes.</returns>
        public static byte[] ToBytesAsHex(this string s)
        {
            s = s.Replace(" ", string.Empty);
            var buffer = new byte[s.Length / 2];
            for (var i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            }

            return buffer;
        }

        /// <summary>
        /// Convert a string of hex digits to a printable string of characters.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPrintableStringAsHex(this string value)
        {
            byte[] temp = value.ToBytesAsHex();
            var loc = string.Empty;
            for (var i = 0; i < temp.Length; i++)
            {
                loc += temp[i].ToPrintableChar();
            }

            return loc;
        }

        /// <summary>
        /// Converts a integer to a hex string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToHexString(this int value)
        {
            var temp = BitConverter.GetBytes(value);
            return temp.ToHexString();
        }

        /// <summary>
        /// Converts a hex string to a integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToIntAsHex(this string value)
        {
            byte[] bytes = value.ToBytesAsHex();
            if (bytes.Length >= 2)
            {
                return BitConverter.ToInt16(bytes, 0);
            }

            return int.MinValue;
        }
    }
}