// Title: Simple file/screen logging
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
using System.Text;
using System.Threading.Tasks;
using ESH.Utility.ExtensionMethods;

namespace ESH.Utility
{
    /// <summary>
    /// Logs text to a file as well as to the console
    /// </summary>
    /// <example>
    /// <code>
    /// using (LogFile logFile = new LogFile()) { ... }
    /// </code>
    /// </example>
    public class LogFile : IDisposable
    {
        private StreamWriter _Writer;
        /// <summary>
        /// The StreamWriter that is being used
        /// </summary>
        public StreamWriter Writer
        {
            get
            {
                return this._Writer;
            }
        }

        private string _LogPath;
        /// <summary>
        /// The path to the log file. Defaults to a subfolder under the calling file's folder named "Logs"
        /// </summary>
        public string LogPath
        {
            get
            {
                return (string.IsNullOrWhiteSpace(this._LogPath) ? Path.Combine(AssemblyInfo.GetCallingAssemblyPath(), "Logs") : this._LogPath.Trim());
            }
            set
            {
                this._LogPath = value;
            }
        }

        private bool _LogToConsole;
        /// <summary>
        /// Whether or not to log to the console. Can be overriden by the writeToConsole parameter in WriteLine()
        /// </summary>
        public bool LogToConsole
        {
            get { return _LogToConsole; }
            set { _LogToConsole = value; }
        }

        private bool _LogToFile;
        /// <summary>
        /// Whether or not to log to file. Can be overriden by the writeToLog parameter in WriteLine()
        /// </summary>
        public bool LogToFile
        {
            get { return _LogToFile; }
            set
            {
                _LogToFile = value;
                if (value == true)
                {
                    InitializeLogWriter();
                }
                else if (_Writer != null)
                {
                    _Writer.Close();
                    _Writer = null;
                }
            }
        }

        private string _LogTag;
        /// <summary>
        /// Prefix for the log filename. Filename is PREFIX-YYYYMMDD.txt
        /// </summary>
        public string LogTag
        {
            get
            {
                return (string.IsNullOrWhiteSpace(this._LogTag) ? "Log" : this._LogTag.Trim());
            }
            set
            {
                this._LogTag = (string.IsNullOrWhiteSpace(value) ? "Log" : value.Trim());
            }
        }

        /// <summary>
        /// Object that is used to guarantee that only one call is writing to the file at a time
        /// </summary>
        private object _LogLocker = new object();

        /// <summary>
        /// Initializes the log folder and the StreamWriter
        /// </summary>
        private void InitializeLogWriter()
        {
            if (_Writer != null)
            {
                _Writer.Close();
            }

            if (!Directory.Exists(this.LogPath))
                Directory.CreateDirectory(this.LogPath);

            string logFileName = this.LogTag + "-" + DateTime.Now.ToSortableDate() + ".txt";
            _Writer = new StreamWriter(Path.Combine(this.LogPath, logFileName), true);
        }

        /// <summary>
        /// Creates a LogFile object
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="logToFile">Whether or not to log to a file</param>
        /// <param name="logToConsole">Whether or not to log to the console</param>
        public LogFile(Stream stream, bool logToFile = true, bool logToConsole = true)
        {
            this.LogToFile = logToFile;
            this.LogToConsole = logToConsole;
            this._Writer = new StreamWriter(stream);
            this._Writer.AutoFlush = true;
        }

        /// <summary>
        /// Creates a LogFile object
        /// </summary>
        /// <param name="path">Path to the log file</param>
        /// <param name="tag">Tag prefix for the log file</param>
        /// <param name="logToFile">Whether or not to log to a file</param>
        /// <param name="logToConsole">Whether or not to log to the console</param>
        public LogFile(string path, string tag, bool logToFile = true, bool logToConsole = true)
        {
            this.LogPath = path;
            this.LogToConsole = logToConsole;
            this.LogTag = tag;
            this.LogToFile = logToFile;
        }

        /// <summary>
        /// Creates a LogFile object
        /// </summary>
        /// <param name="logToFile">Whether or not to log to a file</param>
        /// <param name="logToConsole">Whether or not to log to the console</param>
        public LogFile(bool logToFile = true, bool logToConsole = true)
        {
            this.LogToFile = logToFile;
            this.LogToConsole = logToConsole;
        }

        /// <summary>
        /// Utility function to clear the current console line
        /// </summary>
        public static void ClearCurrentConsoleLine()
        {
            Console.Write("\r");
            for (int i = 0; i < Console.WindowWidth - 1; i++)
                Console.Write(" ");
            Console.Write("\r");
        }

        /// <summary>
        /// Writes a message to the log/console
        /// </summary>
        /// <param name="message">Message to write</param>
        public void WriteLine(string message = "")
        {
            string callingClassName;
            try
            {
                callingClassName = ReflectionTools.GetCallingType().Name;
            }
            catch
            {
                callingClassName = "Unknown";
            }

            string callingMethodName;
            try
            {
                callingMethodName = ReflectionTools.GetCallingMethod().Name;
            }
            catch
            {
                callingMethodName = "Unknown";
            }
            WriteLine(message, this.LogToFile, this.LogToConsole, callingClassName, callingMethodName);
        }

        /// <summary>
        /// Writes a message to the log/console
        /// </summary>
        /// <param name="message">Mesage to write</param>
        /// <param name="callingClassName">Class that called the function</param>
        /// <param name="callingMethodName">Method that called the function</param>
        public void WriteLine(string message, string callingClassName = null, string callingMethodName = null)
        {
            if (callingClassName == null)
            {
                try
                {
                    callingClassName = ReflectionTools.GetCallingType().Name;
                }
                catch
                {
                    callingClassName = "Unknown";
                }
            }

            if (callingMethodName == null)
            {
                try
                {
                    callingMethodName = ReflectionTools.GetCallingMethod().Name;
                }
                catch
                {
                    callingMethodName = "Unknown";
                }
            }
            WriteLine(message, this.LogToFile, this.LogToConsole, callingClassName, callingMethodName);
        }

        /// <summary>
        /// Writes a message to the log/console
        /// </summary>
        /// <param name="message">Mesage to write</param>
        /// <param name="logToFile">Whether or not to write to the log</param>
        /// <param name="logToConsole">Whether or not to write to the console</param>
        /// <param name="callingClassName">Class that called the function</param>
        /// <param name="callingMethodName">Method that called the function</param>
        public void WriteLine(string message, bool? logToFile = null, bool? logToConsole = null, string callingClassName = null, string callingMethodName = null)
        {
            if (message == null)
            {
                message = "";
            }

            if (callingClassName == null)
            {
                try
                {
                    callingClassName = ReflectionTools.GetCallingType().Name;
                }
                catch
                {
                    callingClassName = "Unknown";
                }
            }

            if (callingMethodName == null)
            {
                try
                {
                    callingMethodName = ReflectionTools.GetCallingMethod().Name;
                }
                catch
                {
                    callingMethodName = "Unknown";
                }
            }

            if (!logToConsole.HasValue)
            {
                logToConsole = this.LogToConsole;
            }

            if (logToConsole.Value)
            {
                Console.WriteLine(Iso8601.ToIso8601String(DateTime.Now) + "/" + callingClassName + "." + callingMethodName + "/" + message);
            }

            if (!logToFile.HasValue)
            {
                logToFile = this.LogToFile;
            }

            if (logToFile.Value)
            {
                lock (this._LogLocker)
                {
                    if (this._Writer == null)
                    {
                        InitializeLogWriter();
                    }

                    this._Writer.WriteLine(Iso8601.ToIso8601String(DateTime.Now) + "/" + callingClassName + "." + callingMethodName + "/" + message);
                    this._Writer.Flush();
                }
            }
        }

        /// <summary>
        /// Flushes the stream
        /// </summary>
        public void Flush()
        {
            if (this._Writer != null)
            {
                this._Writer.Flush();
            }
        }

        /// <summary>
        /// Disposes of the stream
        /// </summary>
        // Propagate Dispose to StreamWriter
        public void Dispose()
        {
            if (this._Writer != null)
            {
                this._Writer.Dispose();
            }
        }
    }
}