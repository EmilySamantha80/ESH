// Title:  BaseX math (Math for numbers with any character set)
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

namespace ESH.Utility
{
    public class BaseX
    {
        // Built-in BaseX alphabets
        public const string Base2Alphabet = "01";
        public const string Base10Alphabet = "0123456789";
        public const string Base16Alphabet = "0123456789ABCDEF";
        public const string Base36Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Base62Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Converts an Int64 into a BaseX number
        /// </summary>
        /// <param name="input">Int64 number to convert</param>
        /// <param name="baseAlphabet">BaseX alphabet to use</param>
        /// <returns>BaseX result</returns>
        public static string FromInt64(Int64 input, string baseAlphabet)
        {
            char[] baseChars = baseAlphabet.ToCharArray();
            var result = new Stack<char>();
            bool isNegative = false;
            if (input < 0)
            {
                isNegative = true;
                input = input * -1;
            }

            while (input != 0)
            {
                result.Push(baseChars[input % baseAlphabet.Length]);
                input /= baseAlphabet.Length;
            }

            string resultString = new string(result.ToArray());
            if (isNegative)
            {
                resultString = "-" + resultString;
            }
            return resultString;
        }

        /// <summary>
        /// Converts a BaseX number into an Int64
        /// </summary>
        /// <param name="input">BaseX number to convert</param>
        /// <param name="baseAlphabet">BaseX alphabet to use</param>
        /// <returns>Int64 result</returns>
        public static Int64 ToInt64(string input, string baseAlphabet)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "Argument must not be null.");
            }

            Int64 result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != '-')
                {
                    char c = input[i];
                    if (baseAlphabet.IndexOf(c) == -1)
                    {
                        throw new ArgumentOutOfRangeException("input", "Argument contains invalid characters.");
                    }
                    result = result + (Int64)(baseAlphabet.IndexOf(c) * Math.Pow(baseAlphabet.Length, input.Length - i - 1));
                }
            }

            if (input[0] == '-')
            {
                result = result * -1;
            }

            return result;
        }

        /// <summary>
        /// Adds a specified integer to a BaseX number
        /// </summary>
        /// <param name="number">BaseX number to convert</param>
        /// <param name="i">Integer to add</param>
        /// <param name="baseAlphabet">BaseX alphabet to use</param>
        /// <returns>Base36 result</returns>
        public static string Add(string number, Int64 i, string baseAlphabet)
        {
            return FromInt64(ToInt64(number, baseAlphabet) + i, baseAlphabet);
        }

        /// <summary>
        /// Subtracts a specified integer from a BaseX number
        /// </summary>
        /// <param name="number">BaseX number to convert</param>
        /// <param name="i">Integer to subtract</param>
        /// <param name="baseAlphabet">BaseX alphabet to use</param>
        /// <returns>Base36 result</returns>
        public static string Subtract(string number, Int64 i, string baseAlphabet)
        {
            return FromInt64(ToInt64(number, baseAlphabet) - i, baseAlphabet);
        }

        /// <summary>
        /// Multiplies a specified integer and a BaseX number
        /// </summary>
        /// <param name="number">BaseX number to convert</param>
        /// <param name="i">Integer to subtract</param>
        /// <param name="baseAlphabet">BaseX alphabet to use</param>
        /// <returns>Base36 result</returns>
        public static string Multiply(string number, Int64 i, string baseAlphabet)
        {
            return FromInt64(ToInt64(number, baseAlphabet) * i, baseAlphabet);
        }

        /// <summary>
        /// Divides a BaseX number by a specified integer (without remainder)
        /// </summary>
        /// <param name="number">BaseX number to convert</param>
        /// <param name="i">Integer to subtract</param>
        /// <param name="baseAlphabet">BaseX alphabet to use</param>
        /// <returns>Base36 result</returns>
        public static string Divide(string number, Int64 i, string baseAlphabet)
        {
            return FromInt64(ToInt64(number, baseAlphabet) / i, baseAlphabet);
        }
    }
}