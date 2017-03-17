// Title:  Format exceptions as detailed strings
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

namespace ESH.Utility
{
    public static class ExceptionFormatter
    {
        public static string ExceptionAsString(Exception ex, bool useHtmlNewline = false)
        {
            StringBuilder errorDetail = new StringBuilder();
            if (ex != null)
            {
                Exception orgEx = ex;
                errorDetail.Append("[Exceptions]: ");
                errorDetail.Append(Environment.NewLine);
                int i = 1;
                while (orgEx != null)
                {
                    errorDetail.Append("   #" + i++.ToString() + ": ");
                    errorDetail.Append("(" + orgEx.GetType().ToString() + ") " + orgEx.Message);
                    errorDetail.Append(Environment.NewLine);
                    orgEx = orgEx.InnerException;
                    if (orgEx != null && orgEx.GetType() == typeof(AggregateException))
                    {
                        orgEx = orgEx.InnerException;
                    }
                }
                while (ex != null)
                {
                    errorDetail.Append(Environment.NewLine);
                    if (ex.GetType() == typeof(AggregateException))
                    {
                        ex = ((AggregateException)ex).Flatten().InnerException;
                    }
                    errorDetail.Append(ToLogString(ex: ex));
                    ex = ex.InnerException;
                }
            }
            else
            {
                errorDetail.Append(ToLogString(new Exception()));
            }

            if (useHtmlNewline)
                errorDetail.Replace(Environment.NewLine, "<br />");

            return errorDetail.ToString();
        }

        /// <summary>
        /// <para>Creates a log-string from the Exception.</para>
        /// <para>The result includes the stacktrace, innerexception et cetera, separated by <seealso cref="Environment.NewLine"/>.</para>
        /// </summary>
        /// <param name="ex">The exception to create the string from.</param>
        /// <param name="additionalMessage">Additional message to place at the top of the string, maybe be empty or null.</param>
        /// <returns></returns>
        private static string ToLogString(Exception ex, string additionalMessage = "")
        {
            StringBuilder msg = new StringBuilder();

            if (!string.IsNullOrEmpty(additionalMessage))
            {
                msg.Append(additionalMessage);
                msg.Append(Environment.NewLine);
            }

            if (ex != null)
            {
                try
                {
                    Exception orgEx = ex;

                    msg.Append("[Excpetion]: (");
                    msg.Append(ex.GetType().ToString());
                    msg.Append(") " + ex.Message);
                    msg.Append(Environment.NewLine);

                    if (ex.Source != null)
                    {
                        msg.Append("[Source]: ");
                        msg.Append(ex.Source);
                        msg.Append(Environment.NewLine);
                    }

                    if (ex.TargetSite != null)
                    {
                        msg.Append("[TargetSite]: ");
                        msg.Append(ex.TargetSite.ToString());
                        msg.Append(Environment.NewLine);
                    }

                    //if (ex.GetType() == typeof(WebApiException)) {
                    //    WebApiException apiEx = (WebApiException)ex;
                    //    msg.Append("[WebApiInfo]");
                    //    msg.Append(Environment.NewLine);
                    //    msg.AppendLine("   BaseAddress: " + apiEx.ConnectionInfo.BaseAddress.ToString());
                    //    msg.AppendLine("   ApiPath: " + apiEx.ApiPath.ToSafeString());
                    //    msg.AppendLine("   Username: " + apiEx.ConnectionInfo.Username.ToSafeString());
                    //    if (apiEx.ResponseMessage != null) {
                    //        msg.AppendLine("   ResponseCode: " + apiEx.ResponseMessage.StatusCode.ValueString() + " (" + apiEx.ResponseMessage.ReasonPhrase + ")");
                    //    }

                    //    if (apiEx.ApiParameters != null) {
                    //        msg.AppendLine("   ApiParameters: " + apiEx.ApiParameters.Explode());
                    //   }
                    //}

                    if (ex.Data != null)
                    {
                        foreach (DictionaryEntry entry in ex.Data)
                        {
                            msg.Append("[Data]: ");
                            msg.Append(entry.Key.ToString());
                            msg.Append(" = ");
                            if (entry.Value != null)
                            {
                                msg.Append(entry.Value.ToString());
                            }
                            else
                            {
                                msg.Append("<null>");
                            }
                            msg.Append(Environment.NewLine);
                        }
                    }

                    if (ex.StackTrace != null)
                    {
                        msg.Append("[StackTrace]: ");
                        msg.Append(Environment.NewLine);
                        msg.Append(ex.StackTrace.ToString());
                        msg.Append(Environment.NewLine);
                    }

                    //                    Exception baseException = ex.GetBaseException();
                    //                    if (baseException != null && ex.GetBaseException() != ex) {
                    //                        msg.Append("[BaseException]: ");
                    //                        msg.Append(Environment.NewLine);
                    //                        msg.Append(ex.GetBaseException());
                    //                    }
                }
                finally
                {
                }
            }
            return msg.ToString();
        }
    }
}
