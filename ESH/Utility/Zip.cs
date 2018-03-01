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
