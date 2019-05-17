// Title: HTTP tools
// Author: Emily Heiner (emilysamantha80@gmail.com)
//
// MIT License
// Copyright(c) 2019 Emily Heiner
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility
{
    public class Http
    {
        /// <summary>
        /// Object that holds the response of HTTP operations
        /// </summary>
        public class HttpResponse
        {
            public string Content { get; set; }
            public HttpStatusCode StatusCode { get; set; }
        }

        /// <summary>
        /// Perofrms an HTTP Post and returns the results
        /// </summary>
        /// <param name="uri">URI to post to</param>
        /// <param name="postVars">Key/Value POST variables</param>
        /// <param name="userAgent">User agent string</param>
        /// <returns>HttpResponse object containing the results of the web request</returns>
        public static async Task<HttpResponse> DoHttpPostAsync(Uri uri, Dictionary<string, string> postVars, string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36")
        {
            var result = new HttpResponse();

            var formContent = new FormUrlEncodedContent(postVars);
            var client = new HttpClient();

            using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                request.Content = formContent;
                request.Headers.ExpectContinue = false;
                request.Headers.Add("cache-control", "max-age=0");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Add("user-agent", userAgent);
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    //Console.WriteLine("Response Code: " + ((int)response.StatusCode).ToString() + " " + response.StatusCode.ToString());

                    result.Content = await response.Content.ReadAsStringAsync();
                    result.StatusCode = response.StatusCode;
                }
            }

            return result;

        }
    }
}
