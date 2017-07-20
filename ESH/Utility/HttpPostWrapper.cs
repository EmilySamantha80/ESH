// Title:  HTTP POST Wrapper
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility
{
    /// <summary>
    /// Wrapper for HTTP POST requests
    /// </summary>
    public class HttpPostWrapper
    {
        private HttpWebRequest _WrappedHttpWebRequest;
        private Uri _WrappedUri;
        private string _PostData;

        /// <summary>
        /// Gets the wrapped HttpWebRequest
        /// </summary>
        public HttpWebRequest WrappedHttpWebRequest
        {
            get
            {
                return _WrappedHttpWebRequest;
            }
        }

        /// <summary>
        /// Gets or sets the wrapped Uri. The Wrapped HttpWebRequest object is reset when this is changed.
        /// </summary>
        public Uri WrappedUri
        {
            get
            {
                return _WrappedUri;
            }
            set
            {
                _WrappedHttpWebRequest = (HttpWebRequest)WebRequest.Create(value);
            }
        }

        /// <summary>
        /// Gets or sets the method for the request
        /// </summary>
        public string Method
        {
            get
            {
                return CleanString(_WrappedHttpWebRequest.Method);
            }
            set
            {
                _WrappedHttpWebRequest.Method = CleanString(value);
            }
        }

        /// <summary>
        /// Gets or sets the content type for the request
        /// </summary>
        public string ContentType
        {
            get
            {
                return CleanString(_WrappedHttpWebRequest.ContentType);
            }
            set
            {
                _WrappedHttpWebRequest.ContentType = CleanString(value);
            }
        }

        /// <summary>
        /// Gets or sets the POST data for the request
        /// </summary>
        public string PostData
        {
            get
            {
                return CleanString(_PostData);
            }
            set
            {
                _PostData = CleanString(value);
            }
        }

        /// <summary>
        /// Initializes the HttpWrapper object
        /// </summary>
        public HttpPostWrapper()
        {
            _WrappedUri = new Uri(string.Empty);
            Initialize();
        }

        /// <summary>
        /// Initializes the HttpWrapper object
        /// </summary>
        /// <param name="uri">URI of the request</param>
        public HttpPostWrapper(Uri uri)
        {
            _WrappedUri = uri;
            Initialize();
        }

        private void Initialize()
        {
            _WrappedHttpWebRequest = (HttpWebRequest)WebRequest.Create(_WrappedUri);
        }

        private static string CleanString(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return string.Empty;
            }
            else
            {
                return inputString.ToString();
            }
        }

        /// <summary>
        /// Adds POST data to the web request
        /// </summary>
        /// <param name="varName">Variable name</param>
        /// <param name="varValue">Variable value</param>
        public void AddPostDataVar(string varName, string varValue)
        {
            if (!string.IsNullOrEmpty(_PostData))
            {
                _PostData += "&";
            }
            _PostData += CleanString(varName) + "=" + CleanString(varValue);
        }

        /// <summary>
        /// Performs the HTTP POST request
        /// </summary>
        /// <returns>Byte array containing the response</returns>
        public byte[] DoHttpPost()
        {
            byte[] result;
            byte[] buffer = new byte[4096];

            this.Method = "POST";
            this.ContentType = "application/x-www-form-urlencoded";

            try
            {
                using (StreamWriter sw = new StreamWriter(_WrappedHttpWebRequest.GetRequestStream()))
                {
                    sw.Write(_PostData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error posting HTTP Request! " + ex.Message + "\r\n" + ex.StackTrace);
            }
            try
            {
                using (WebResponse response = _WrappedHttpWebRequest.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            int count = 0;
                            do
                            {
                                count = responseStream.Read(buffer, 0, buffer.Length);
                                memoryStream.Write(buffer, 0, count);
                            } while (count > 0);

                            result = memoryStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return result;
        }
    }
}
