// Title:  HTML Tools
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

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESH.Utility
{
    public class HtmlTools
    {
        /// <summary>
        /// Removes all tags and attributes from html except those specified as being allowed.
        /// </summary>
        /// <param name="html">HTML document to modify</param>
        /// <param name="allowedTags">Tags that will not be removed.</param>
        /// <param name="allowedAttributes">Attributes that will not be removed from allowed tags.</param>
        /// <returns>Modified HTML document</returns>
        /// <remarks>If a tag is not allowed, any tags (including allowed tags) within that tag will not be included.</remarks>
        public static string StripTags(string html, StringCollection allowedTags, StringCollection allowedAttributes)
        {
            HtmlDocument hd = new HtmlDocument();
            hd.LoadHtml(html);

            if (hd == null) return string.Empty;

            string output = string.Empty;
            foreach (HtmlNode node in hd.DocumentNode.ChildNodes)
            {
                output += DoStripTags(node, allowedTags, allowedAttributes);
            }

            return output;
        }

        private static string DoStripTags(HtmlNode hn, StringCollection allowedTags, StringCollection allowedAttributes)
        {
            if (hn == null) return string.Empty;
            if (hn.Name == "#text")
                return hn.InnerText;
            if (!allowedTags.Contains(hn.Name))
                return string.Empty;

            StringBuilder s = new StringBuilder();
            s.Append("<").Append(hn.Name);

            foreach (var att in hn.Attributes)
            {
                if (allowedAttributes.Contains(att.Name))
                {
                    s.Append(" ").Append(att.Name).Append("=\"").Append(att.Value).Append("\"");
                }
            }

            s.Append(">");

            if (hn.HasChildNodes)
            {
                foreach (HtmlNode node in hn.ChildNodes)
                {
                    s.Append(DoStripTags(node, allowedTags, allowedAttributes));
                }
            }

            string pattern = "^<" + hn.Name + ".*</" + hn.Name + ">$";
            if (Regex.IsMatch(hn.OuterHtml, pattern, RegexOptions.Singleline)) // If there was a closing tag add it back.
                s.Append("</").Append(hn.Name).Append(">");

            return s.ToString();
        }
    }
}
