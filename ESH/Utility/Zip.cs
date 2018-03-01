// Title:  ZIP file manipulation
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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility
{
    public static class Zip
    {
        /// <summary>
        /// Creates a ZIP archive from a list
        /// </summary>
        /// <param name="sourceEntries">Filenames and content to ZIP</param>
        /// <param name="compressionLevel">ZIP compression level</param>
        /// <returns>Bytes of the ZIP file</returns>
        public static byte[] CreateArchiveFromList(Dictionary<string, byte[]> sourceEntries, CompressionLevel compressionLevel)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                try
                {
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var entry in sourceEntries)
                        {
                            var zipEntry = archive.CreateEntry(entry.Key, compressionLevel);

                            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(zipEntry.Open()))
                            {

                                try
                                {
                                    writer.Write(entry.Value);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }

                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return memoryStream.ToArray();
            }
        }


        /// <summary>
        /// Creates a ZIP archive from a list
        /// </summary>
        /// <param name="sourceEntries">Filenames and content to ZIP</param>
        /// <param name="compressionLevel">ZIP compression level</param>
        /// <returns></returns>
        public static void CreateArchiveFromList(string destinationZipFile, Dictionary<string, byte[]> sourceEntries, CompressionLevel compressionLevel)
        {
            using (System.IO.FileStream fs = File.Open(destinationZipFile, System.IO.FileMode.Create))
            {
                try
                {
                    using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Create, true))
                    {
                        foreach (var entry in sourceEntries)
                        {
                            var zipEntry = archive.CreateEntry(entry.Key, compressionLevel);

                            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(zipEntry.Open()))
                            {

                                try
                                {
                                    writer.Write(entry.Value);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

        }

    }
}
