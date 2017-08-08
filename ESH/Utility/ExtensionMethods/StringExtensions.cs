// Title: Utility Extension Methods
// Author: Emily Heiner
//
// MIT License
// Copyright(c) 2017 Emily Heiner (emilysamantha80@gmail.com)
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
        public static string Left(this string input, int length)
        {
            if (input.Length >= length)
            {
                char[] cString = input.ToCharArray();
                Array.Resize(ref cString, length);
                string val = new string(cString);
                return val;
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Get substring of specified number of characters on the right.
        /// </summary>
        public static string Right(this string input, int length)
        {
            if (input.Length >= length)
            {
                char[] cString = input.ToCharArray();
                Array.Reverse(cString);
                Array.Resize(ref cString, length);
                Array.Reverse(cString);
                string val = new string(cString);
                return val;
            }
            else
            {
                return input;
            }
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
            var sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                sb.Append(input[input.Length - i - 1]);
            }

            return sb.ToString();
        }
    }
}
