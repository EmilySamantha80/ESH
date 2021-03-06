﻿// Title: Utility Extension Methods
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
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a DateTime input to a String representation in yyyyMMddHHmmss format
        /// </summary>
        public static string ToSortableDateTime(this DateTime input)
        {
            string sortableFormat = "yyyyMMddHHmmss";

            return input.ToString(sortableFormat);
        }

        /// <summary>
        /// Converts a DateTime input to a String representation in yyyyMMdd format
        /// </summary>
        public static string ToSortableDate(this DateTime input)
        {
            string sortableFormat = "yyyyMMdd";

            return input.ToString(sortableFormat);
        }
    }
}
