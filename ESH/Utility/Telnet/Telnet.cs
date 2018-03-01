// Title:  Minimalist telnet client with scripting capabilities
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
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

/// <summary>
/// Minimalist telnet implementation
/// </summary>
namespace ESH.Utility.Telnet
{
    /// <summary>
    /// Telnet protocol verbs
    /// </summary>
    public enum TelnetVerbs
    {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    /// <summary>
    /// Telnet protocol options
    /// </summary>
    public enum TelnetOptions
    {
        SGA = 3
    }

    /// <summary>
    /// Minimalist telnet client with scripting capabilities
    /// </summary>
    public class TelnetClient
    {
        private TcpClient _Client;
        /// <summary>
        /// The underlying TcpClinet object
        /// </summary>
        public TcpClient Client { get { return this._Client; } }

        private string _Host;
        /// <summary>
        /// Host name/ip of the telnet server
        /// </summary>
        public string Host { get { return _Host; } }

        private int _Port;
        /// <summary>
        /// Port number of the telnet server
        /// </summary>
        public int Port { get { return _Port; } }

        /// <summary>
        /// Internal variable used for while reading commands coming in from the server.
        /// </summary>
        private int _ReadTimeoutMs { get; set; }


        private StringBuilder _ResponseBuffer;
        /// <summary>
        /// Holds the response from the server.
        /// </summary>
        public string ResponseBuffer
        {
            get
            {
                if (this._ResponseBuffer == null)
                {
                    return string.Empty;
                }
                return this._ResponseBuffer.ToString();
            }
        }

        /// <summary>
        /// Initializes a new telnet client.
        /// </summary>
        /// <param name="host">Host name/ip</param>
        /// <param name="port">Port number</param>
        public TelnetClient(string host, int port = 23)
        {
            this._Client = new TcpClient();
            this._Host = host;
            this._Port = port;
            this._ReadTimeoutMs = 1000;
            this._ResponseBuffer = new StringBuilder();
        }

        /// <summary>
        /// Connect to the server.
        /// </summary>
        public void Connect()
        {
            this._Client.Connect(this.Host, this.Port);
        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        public void Disconnect()
        {
            this._Client.Close();
        }

        /// <summary>
        /// Returns whether or not the client is connected to the server.
        /// NOTE: This function is currently not reliable. It does not detect disconnects if the server closes the connection.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                // We can't use TcpClient.Connected to detect connection state reliably. It lies and says it's still connected sometimes.

                // Find out if the client is still connected to the server.
                // This will catch server side disconnects.
                if (this._Client.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buff = new byte[1];
                    if (this._Client.Client.Receive(buff, SocketFlags.Peek) == 0)
                    {
                        // Server has disconnected
                        return false;
                    }
                }

                // Check if the server has disconnected or has never connected
                if (!this._Client.Connected)
                {
                    return false;
                }

                // Connection is good
                return true;
            }
        }

        /// <summary>
        /// Overly simplified login prompt detection. Shows how to use the scripting capabilities.
        /// </summary>
        /// <param name="username">Username to connect to the server</param>
        /// <param name="password">Password to connect to the server</param>
        /// <param name="promptTimeoutMs">How long to wait for a response from the server</param>
        /// <param name="buffer">String buffer to store the response from the server</param>
        /// <param name="clearBufferAfterRead">Whether or not to clear the buffer after reading the response from the server</param>
        public void Login(string username, string password, int promptTimeoutMs, out string buffer, bool clearBufferAfterRead = true)
        {
            bool result = false;
            string tmpBuffer;
            buffer = "";

            if (!this.IsConnected)
            {
                this.Connect();
            }

            result = this.WaitForConnect(promptTimeoutMs);
            if (!result)
                throw new Exception("Failed to connect to server.");

            result = WaitFor(":", promptTimeoutMs, out tmpBuffer);
            if (!result)
                throw new Exception("Failed to log in! No login prompt.");
            WriteLine(username);
            buffer += tmpBuffer;

            result = WaitFor(":", promptTimeoutMs, out tmpBuffer);
            if (!result)
                throw new Exception("Failed to log in! No password prompt.");
            WriteLine(password);
            buffer += tmpBuffer;

            if (clearBufferAfterRead)
            {
                this.ClearResponseBuffer();
            }
            return;
        }

        /// <summary>
        /// Take the contents of the response buffer, and then clear it.
        /// </summary>
        /// <returns></returns>
        public string TakeResponeBuffer()
        {
            string buffer = this.ResponseBuffer;
            this.ClearResponseBuffer();
            return buffer;
        }

        /// <summary>
        /// Clear the text response buffer
        /// </summary>
        public void ClearResponseBuffer()
        {
            this._ResponseBuffer = new StringBuilder();
        }

        /// <summary>
        /// Write the specified text to the server.
        /// A newline character is appended to the text.
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            this.Write(text + "\n");
        }

        /// <summary>
        /// Write the specified text to the server.
        /// </summary>
        /// <param name="text">Text to write</param>
        public void Write(string text)
        {
            if (!this.IsConnected)
            {
                throw new SocketException((int)SocketError.NotConnected);
            }
            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(text.Replace("\0xFF", "\0xFF\0xFF"));
            _Client.GetStream().Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Read and parse a byte from the receive stream, if there is anything to receive.
        /// </summary>
        /// <returns></returns>
        private int ReadByte()
        {
            if (!this.IsConnected)
            {
                throw new SocketException((int)SocketError.NotConnected);
            }

            int charCode = -1;
            if (_Client.Available > 0)
            {
                charCode = _Client.GetStream().ReadByte();
                charCode = this.ParseTelnetResponseByte(charCode);
                if (charCode > 0)
                {
                    this._ResponseBuffer.Append((char)charCode);
                }
            }
            return charCode;
        }

        /// <summary>
        /// Parse a byte of the response from the server.
        /// </summary>
        /// <param name="charCode">Character code of the response byte</param>
        /// <returns></returns>
        private int ParseTelnetResponseByte(int charCode)
        {
            DateTime timeout = DateTime.Now.AddMilliseconds(this._ReadTimeoutMs);

            switch (charCode)
            {
                case -1:
                    break;
                case (int)TelnetVerbs.IAC:
                    // interpret as command
                    int inputVerb = -1;
                    timeout = DateTime.Now.AddMilliseconds(this._ReadTimeoutMs);
                    while (DateTime.Now < timeout)
                    {
                        if (_Client.Available == 0)
                        {
                            System.Threading.Thread.Sleep(10);
                            continue;
                        }
                        inputVerb = _Client.GetStream().ReadByte();
                        if (inputVerb >= 0)
                        {
                            break;
                        }
                    }
                    if (inputVerb == -1) break;
                    switch (inputVerb)
                    {
                        case (int)TelnetVerbs.IAC:
                            // Literal IAC = 255 escaped, so append char 255 to string
                            return -1;
                        case (int)TelnetVerbs.DO:
                        case (int)TelnetVerbs.DONT:
                        case (int)TelnetVerbs.WILL:
                        case (int)TelnetVerbs.WONT:
                            // Reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                            int inputOption = -1;
                            timeout = DateTime.Now.AddMilliseconds(this._ReadTimeoutMs);
                            while (DateTime.Now < timeout)
                            {
                                inputOption = _Client.GetStream().ReadByte();
                                if (inputOption >= 0)
                                {
                                    break;
                                }
                                System.Threading.Thread.Sleep(10);
                            }
                            if (inputOption == -1) break;
                            _Client.GetStream().WriteByte((byte)TelnetVerbs.IAC);
                            if (inputOption == (int)TelnetOptions.SGA)
                            {
                                _Client.GetStream().WriteByte(inputVerb == (int)TelnetVerbs.DO ? (byte)TelnetVerbs.WILL : (byte)TelnetVerbs.DO);
                            }
                            else
                            {
                                _Client.GetStream().WriteByte(inputVerb == (int)TelnetVerbs.DO ? (byte)TelnetVerbs.WONT : (byte)TelnetVerbs.DONT);
                            }
                            _Client.GetStream().WriteByte((byte)inputOption);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    return charCode;

            }
            return -1;
        }

        /// <summary>
        /// Wait for the specified response from the server.
        /// </summary>
        /// <param name="regex">The text to wait for</param>
        /// <param name="timeoutMs">Wait timeout in ms</param>
        /// <param name="buffer">Holds text received while waiting</param>
        /// <param name="clearBufferAfterRead">Whether or not to clear the buffer after reading the response from the server</param>
        /// <returns></returns>
        public bool WaitFor(string text, int timeoutMs, out string buffer, bool clearBufferAfterRead = true)
        {
            var timeout = DateTime.Now.AddMilliseconds(timeoutMs);
            bool result = false;
            while (DateTime.Now < timeout)
            {
                this.ReadByte();
                if (this.ResponseBuffer.Contains(text))
                {
                    result = true;
                    break;
                }
            }
            buffer = this.ResponseBuffer;
            if (clearBufferAfterRead)
            {
                this.ClearResponseBuffer();
            }
            return result;
        }


        /// <summary>
        /// Wait for the specified response from the server.
        /// </summary>
        /// <param name="regex">The regex to wait for</param>
        /// <param name="timeoutMs">Wait timeout in ms</param>
        /// <param name="buffer">Holds text received while waiting</param>
        /// <param name="clearBufferAfterRead">Whether or not to clear the buffer after reading the response from the server</param>
        /// <returns></returns>
        public bool WaitFor(Regex regex, int timeoutMs, out string buffer, bool clearBufferAfterRead = true)
        {
            var timeout = DateTime.Now.AddMilliseconds(timeoutMs);
            bool result = false;
            while (DateTime.Now < timeout)
            {
                this.ReadByte();
                if (regex.IsMatch(this.ResponseBuffer))
                {
                    result = true;
                    break;
                }
            }
            buffer = this.ResponseBuffer;
            if (clearBufferAfterRead)
            {
                this.ClearResponseBuffer();
            }
            return result;
        }

        /// <summary>
        /// Wait for the specified response from the server.
        /// The response buffer is never cleared after this function.
        /// </summary>
        /// <param name="text">The text to wait for</param>
        /// <param name="timeoutMs">Wait timeout in ms</param>
        /// <returns></returns>
        public bool WaitFor(string text, int timeoutMs)
        {
            string buffer;
            return this.WaitFor(text, timeoutMs, out buffer, false);
        }

        /// <summary>
        /// Wait for the specified response from the server.
        /// The response buffer is never cleared after this function.
        /// </summary>
        /// <param name="regex">The regex to wait for</param>
        /// <param name="timeoutMs">Wait timeout in ms</param>
        /// <returns></returns>
        public bool WaitFor(Regex regex, int timeoutMs)
        {
            string buffer;
            return this.WaitFor(regex, timeoutMs, out buffer, false);
        }

        /// <summary>
        /// Wait for the server to disconnect from the client.
        /// </summary>
        /// <param name="timeoutMs">Wait timeout in ms</param>
        /// <returns></returns>
        public bool WaitForDisconnect(int timeoutMs)
        {
            var timeout = DateTime.Now.AddMilliseconds(timeoutMs);
            while (DateTime.Now < timeout)
            {
                if (!this.IsConnected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Wait for a connection to the server to be established
        /// </summary>
        /// <param name="timeoutMs">Wait timeout in ms</param>
        /// <returns></returns>
        public bool WaitForConnect(int timeoutMs)
        {
            var timeout = DateTime.Now.AddMilliseconds(timeoutMs);
            while (DateTime.Now < timeout)
            {
                if (this.IsConnected)
                {
                    return true;
                }
            }
            return false;
        }
    }
}