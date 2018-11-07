// Title: HTTP Tools
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
using System.Web.UI;

namespace ESH.Utility
{
    public class HttpTools
    {
        /// <summary>
        /// POST data and Redirect to the specified url using the specified page.
        /// </summary>
        /// <param name="page">The page which will be the referrer page.</param>
        /// <param name="destinationUrl">The destination Url to which
        /// the post and redirection is occuring.</param>
        /// <param name="data">The data should be posted.</param>
        /// <Author>Samer Abu Rabie</Author>
        /// <see cref="http://www.codeproject.com/Articles/37539/Redirect-and-POST-in-ASP-NET"/>
        public static string RedirectAndPOST(Page page, string destinationUrl, Dictionary<string, string> data)
        {
            //Prepare the Posting form
            string strForm = PreparePOSTForm(destinationUrl, data);
            //Add a literal control the specified page holding 
            //the Post Form, this is to submit the Posting form with the request.
            if (page != null)
            {
                page.Controls.Add(new LiteralControl(strForm));
            }

            return strForm;
        }

        /// <summary>
        /// This method prepares an Html form which holds all data
        /// in hidden field in the addetion to form submitting script.
        /// </summary>
        /// <param name="url">The destination Url to which the post and redirection
        /// will occur, the Url can be in the same App or ouside the App.</param>
        /// <param name="data">A collection of data that
        /// will be posted to the destination Url.</param>
        /// <returns>Returns a string representation of the Posting form.</returns>
        /// <Author>Samer Abu Rabie</Author>
        public static String PreparePOSTForm(string url, Dictionary<string, string> data)
        {
            //Set a name for the form
            string formID = "PostForm";
            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" +
                           formID + "\" action=\"" + url +
                           "\" method=\"POST\">");

            foreach (var key in data)
            {
                strForm.Append("<input type=\"hidden\" name=\"" + key.Key +
                               "\" value=\"" + key.Value + "\">");
            }

            strForm.Append("</form>");
            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language=\"javascript\">");
            strScript.Append("var v" + formID + " = document." +
                             formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }
    }
}
