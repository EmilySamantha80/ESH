// Title: Simple file/screen logging
// Author: Emily Heiner
// Usage: using (LogFile logFile = new LogFile()) { ... }
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
    public class LogFile : IDisposable
    {
        private StreamWriter _Writer;
        public StreamWriter Writer
        {
            get
            {
                return this._Writer;
            }
        }

        private string _LogPath;
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

        private bool _LogToFile;
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

        private object _LogLocker = new object();

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

        public LogFile(Stream stream, bool logToFile = true)
        {
            this.LogToFile = logToFile;
            this._Writer = new StreamWriter(stream);
            this._Writer.AutoFlush = true;
        }

        public LogFile(string path, string tag, bool logToFile = true)
        {
            this.LogPath = path;
            this.LogTag = tag;
            this.LogToFile = logToFile;
        }

        public LogFile(bool logToFile = true)
        {
            this.LogToFile = logToFile;
        }

        public static void ClearCurrentConsoleLine()
        {
            Console.Write("\r");
            for (int i = 0; i < Console.WindowWidth - 1; i++)
                Console.Write(" ");
            Console.Write("\r");
        }

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
            WriteLine(message, this.LogToFile, true, callingClassName, callingMethodName);
        }

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
            WriteLine(message, this.LogToFile, true, callingClassName, callingMethodName);
        }

        public void WriteLine(string message, bool? writeToLog = null, bool writeToConsole = true, string callingClassName = null, string callingMethodName = null)
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

            if (writeToConsole)
            {
                Console.WriteLine(Iso8601.ToIso8601String(DateTime.Now) + "/" + callingClassName + "." + callingMethodName + "/" + message);
                //Console.WriteLine(DateTime.Now.ToIso8601String() + "/" + message);
            }

            if (!writeToLog.HasValue)
            {
                writeToLog = this.LogToFile;
            }

            if (writeToLog.Value)
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

        public void Flush()
        {
            this._Writer.Flush();
        }

        // Propagate Dispose to StreamWriter
        public void Dispose()
        {
            this._Writer.Dispose();
        }
    }
}