// Title:  SFTP utilities 
// Author: Emily Heiner
//
// MIT License
// Copyright(c) 2018 Emily Heiner (emilysamantha80@gmail.com)
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

using Renci.SshNet; //NuGet package SSH.net is required
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility
{
    public class Sftp
    {
        /// <summary>
        /// Upload a file to an SFTP server
        /// </summary>
        /// <param name="host">Hostname/IP</param>
        /// <param name="port">Port</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="rootFolder">Folder to upload the file to</param>
        /// <param name="localFileBytes">Bytes of the local file to upload</param>
        /// <param name="remoteFile">Remote filename</param>
        public static void UploadFileToSftp(string host, int port, string username, string password, string rootFolder, byte[] localFileBytes, string remoteFile)
        {
            //Console.WriteLine("Connecting to " + host + ":" + port + " as " + username);
            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    //Console.WriteLine("Connected to SFTP server");
                    if (rootFolder != null)
                        client.ChangeDirectory(rootFolder);

                    using (var ms = new MemoryStream(localFileBytes))
                    {
                        client.BufferSize = (uint)ms.Length; // bypass Payload error large files
                        client.UploadFile(ms, remoteFile);
                    }
                }
                else
                {
                    //Console.WriteLine("Failed to connect to SFTP server");
                    throw new Exception("Failed to connect to SFTP server");
                }
            }
        }

        /// <summary>
        /// Upload a file to an SFTP server
        /// </summary>
        /// <param name="host">Hostname/IP</param>
        /// <param name="port">Port</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="rootFolder">Folder to upload the file to</param>
        /// <param name="localFile">Local file to upload</param>
        /// <param name="remoteFile">Remote filename</param>
        public static void UploadFileToSftp(string host, int port, string username, string password, string rootFolder, string localFile, string remoteFile)
        {

            //Console.WriteLine("Connecting to " + host + ":" + port + " as " + username);

            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    //Console.WriteLine("Connected to SFTP server");
                    if (rootFolder != null)
                        client.ChangeDirectory(rootFolder);

                    using (var fileStream = new FileStream(localFile, FileMode.Open))
                    {

                        client.BufferSize = 4 * 1024; // bypass Payload error large files
                        client.UploadFile(fileStream, remoteFile);
                    }
                }
                else
                {
                    //Console.WriteLine("Failed to connect to SFTP server");
                    throw new Exception("Failed to connect to SFTP server");
                }
            }
        }
    }
}
