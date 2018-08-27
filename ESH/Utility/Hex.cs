// Title: Hex manipulation
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
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility
{
    public class Hex
    {
        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        /// <returns>A hex string.</returns>
        /// <param name="input">A byte array.</param>
        public static string BytesToHexString(byte[] input)
        {
            return BitConverter.ToString(input).Replace("-", String.Empty);
        }

        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        /// <returns>A byte array.</returns>
        /// <param name="input">A hex string.</param>
        public static byte[] HexStringToBytes(string input)
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
        public static int HexValue(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            else if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            else if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            throw new ArgumentException("Invalid hex digit: " + c);
        }
    }
}
