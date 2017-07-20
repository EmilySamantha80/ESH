// Title: Command line argument parser
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESH.Utility
{
    /// <summary>
    /// Better command line argument parsing
    /// </summary>
    public class CommandLineArgs
    {
        protected enum enumParseState : int { StartToken, InQuote, InToken };

        /// <summary>
        /// Parses the command line arguments into a more logical array of arguments
        /// </summary>
        /// <param name="exeNameAsArg">Whether or not to include the executable file name as an argument</param>
        /// <returns>Array of arguments</returns>
        public static string[] Parse(bool exeNameAsArg = false)
        {
            String CommandLineArgs = Environment.CommandLine.ToString();

            //Console.WriteLine("Command entered: " + CommandLineArgs);

            List<String> listArgs = new List<String>();

            Regex rWhiteSpace = new Regex("[\\s]");
            StringBuilder token = new StringBuilder();
            enumParseState eps = enumParseState.StartToken;

            for (int i = 0; i < CommandLineArgs.Length; i++)
            {
                char c = CommandLineArgs[i];
                //    Console.WriteLine(c.ToString()  + ", " + eps);
                //Looking for beginning of next token
                if (eps == enumParseState.StartToken)
                {
                    if (rWhiteSpace.IsMatch(c.ToString()))
                    {
                        //Skip whitespace
                    }
                    else if (c == '"')
                    {
                        eps = enumParseState.InQuote;
                    }
                    else
                    {
                        token.Append(c);
                        eps = enumParseState.InToken;
                    }
                }
                else if (eps == enumParseState.InToken)
                {
                    if (rWhiteSpace.IsMatch(c.ToString()))
                    {
                        //Console.WriteLine("Token: [" + token.ToString() + "]");
                        listArgs.Add(token.ToString().Trim());
                        eps = enumParseState.StartToken;

                        //Start new token.
                        token.Remove(0, token.Length);
                    }
                    else if (c == '"')
                    {
                        // token.Append(c);
                        eps = enumParseState.InQuote;
                    }
                    else
                    {
                        token.Append(c);
                        eps = enumParseState.InToken;
                    }

                }
                //When in a quote, white space is included in the token
                else if (eps == enumParseState.InQuote)
                {
                    if (c == '"')
                    {
                        // token.Append(c);
                        eps = enumParseState.InToken;
                    }
                    else
                    {
                        token.Append(c);
                        eps = enumParseState.InQuote;
                    }

                }


            }
            if (token.ToString() != "")
            {
                listArgs.Add(token.ToString());
                //Console.WriteLine("Token: [" + token.ToString() + "]");
            }
            if (!exeNameAsArg)
            {
                listArgs.RemoveAt(0);
            }
            return listArgs.ToArray();
        }
    }

}
