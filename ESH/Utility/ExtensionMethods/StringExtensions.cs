// Title: Utility Extension Methods
// Author: Emily Heiner
//
// MIT License
// Copyright(c) 2017-2018 Emily Heiner (emilysamantha80@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility.ExtensionMethods
{
    public static class StringExtensions
    {

        /// <summary>
        /// Converts a string to a byte array.
        /// </summary>
        /// <returns>A byte array.</returns>
        /// <param name="input">A string.</param>
        public static byte[] ToBytes(this string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }

        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        /// <returns>A byte array.</returns>
        /// <param name="input">A hex string.</param>
        public static byte[] HexStringToBytes(this string input)
        {
            if (input.Length % 2 == 1)
                throw new Exception("Hex string must have an even number of digits ");
            byte[] arr = new byte[input.Length >> 1];
            for (int i = 0; i < input.Length >> 1; ++i)
                arr[i] = (byte)((HexValue(input[i << 1]) << 4) + (HexValue(input[(i << 1) + 1])));
            return arr;
        }

        /// <summary>
        /// Returns the hex value for a specified character
        /// </summary>
        private static int HexValue(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            else if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            else if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            throw new ArgumentException("Invalid hex digit: " + c);
        }

        /// <summary>
        /// Returns the string representation of an object, or String.Empty if the object is null
        /// </summary>
        /// <param name="input">Input object</param>
        /// <returns>String representation of the object</returns>
        public static string ToSafeString(this object input)
        {
            return input != null ? input.ToString() : String.Empty;
        }

        /// <summary>
        /// Get substring of specified number of characters on the left.
        /// </summary>
        public static string Left(this string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Length <= maxLength ? input : input.Substring(0, maxLength);
        }

        /// <summary>
        /// Get substring of specified number of characters on the right.
        /// </summary>
        public static string Right(this string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Length <= maxLength ? input : input.Substring(input.Length - maxLength, maxLength);
        }

        /// <summary>
        /// Converts a String to a SecureString
        /// </summary>
        /// <param name="input">String to convert</param>
        /// <returns>SecureString containing the value of the input String</returns>
        public static SecureString ToSecureString(this string input)
        {
            if (input == null)
            {
                return null;
            }
            else
            {
                var output = new SecureString();
                input.ToList().ForEach(output.AppendChar);
                return output;
            }
        }

        /// <summary>
        /// Reverses a string
        /// </summary>
        /// <param name="input">String to reverse</param>
        /// <returns>Reversed string</returns>
        public static string Reverse(this string input)
        {
            char[] c = new char[input.Length];
            int x = 0;
            int y = c.Length - 1;
            while (x <= y)
            {
                c[x] = input[y];
                c[y] = input[x];
                x++;
                y--;
            }
            return new string(c);
        }
    }
}
