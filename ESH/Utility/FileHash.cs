// Title:  MD5 hash generator from files
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
using System.Text;
using System.IO;

namespace ESH.Utility
{
    /// <summary>
    /// Generates MD5 hashes with the ability to save/load the results to/from a file.
    /// </summary>
    public class FileHash
    {
        /// <summary>
        /// FileInfo object containing details of the hashed file
        /// </summary>
        public FileInfo FileDetails { get; set; }

        /// <summary>
        /// MD5 Hash of the file
        /// </summary>
        public string MD5Hash { get; set; }

        /// <summary>
        /// Computes the MD5 hash of a byte array.
        /// </summary>
        /// <param name="inputBytes">Byte array to hash</param>
        /// <returns>The MD5 hash of the byte array</returns>
        public static string ComputeMD5Hash(byte[] inputBytes)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(md5.Hash).Replace("-", "");

        }

        /// <summary>
        /// Computes the hash of a file.
        /// </summary>
        /// <param name="file">Path and filename of the file to hash</param>
        /// <returns>A FileHash object containing the results</returns>
        public static FileHash GetHash(string file)
        {
            var fileHash = new FileHash();

            byte[] inputBytes = File.ReadAllBytes(file);
            fileHash.FileDetails = new FileInfo(file);
            fileHash.MD5Hash = ComputeMD5Hash(inputBytes);

            return fileHash;
        }

        /// <summary>
        /// Computes the hash of multiple files in a directory.
        /// </summary>
        /// <param name="directory">Path of the files to hash</param>
        /// <param name="searchPattern">Search pattern to filter the file list on</param>
        /// <returns>A list of FileHash objects for all matching files</returns>
        public static List<FileHash> GetHashes(string directory, string searchPattern = "*")
        {
            var fileHashes = new List<FileHash>();

            foreach (var file in new DirectoryInfo(directory).GetFiles(searchPattern))
            {
                fileHashes.Add(GetHash(file.FullName));
            }

            return fileHashes;
        }

        /// <summary>
        /// Recomputes the hashes in a hash list from their respective files.
        /// </summary>
        /// <param name="fileHashes"></param>
        /// <returns>An updated list of FileHash objects</returns>
        public static List<FileHash> RefreshHashes(List<FileHash> fileHashes)
        {
            var newHashes = new List<FileHash>();

            foreach (var fileHash in fileHashes)
            {
                try
                {
                    newHashes.Add(GetHash(fileHash.FileDetails.FullName));
                }
                catch { }
            }

            return newHashes;
        }

        /// <summary>
        /// Saves a list of hashes to a file.
        /// </summary>
        /// <param name="fileHashes">List of FileHash objects to save</param>
        /// <param name="hashFile">File to save hashes to</param>
        public static void SaveHashes(List<FileHash> fileHashes, string hashFile)
        {
            if (fileHashes == null)
            {
                throw new ArgumentException("hashList must not be null!");
            }
            if (hashFile == null)
            {
                throw new ArgumentException("hashFile must not be null");
            }

            using (System.IO.FileStream fs = File.Open(hashFile, System.IO.FileMode.Create))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                {
                    foreach (var fileHash in fileHashes)
                    {
                        var line = fileHash.MD5Hash + "  " + fileHash.FileDetails.Name;
                        //Console.WriteLine(line);
                        sw.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Loads a list of hashes from a file
        /// </summary>
        /// <param name="hashFile">File to load hashes from</param>
        /// <returns></returns>
        public static List<FileHash> LoadHashes(string hashFile)
        {
            var fileHashes = new List<FileHash>();
            int count = 0;
            using (System.IO.FileStream fs = File.Open(hashFile, System.IO.FileMode.Open))
            {
                using (System.IO.StreamReader sw = new System.IO.StreamReader(fs))
                {
                    string line;
                    while ((line = sw.ReadLine()) != null)
                    {
                        count++;

                        if (String.IsNullOrWhiteSpace(line))
                            continue;

                        var split = line.IndexOf("  ");
                        if (split <= 0 || split == line.Length - 2) // Catch lines with no split or no text after the split.
                        {
                            throw new InvalidDataException("Malformed hash in line " + count.ToString());
                        }

                        //Console.WriteLine("Length: " + line.Length);
                        //Console.WriteLine("Split: " + split);
                        var lineParts = new string[] { line.Substring(0, split), line.Substring(split + 2, (line.Length - split - 2)) };

                        var fileHash = new FileHash();

                        try
                        {
                            fileHash.MD5Hash = lineParts[0];
                            fileHash.FileDetails = new FileInfo(Path.Combine(new FileInfo(hashFile).Directory.FullName, lineParts[1]));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Malformed has file!", ex);
                        }

                        fileHashes.Add(fileHash);
                    }
                }
            }

            return fileHashes;
        }
    }
}