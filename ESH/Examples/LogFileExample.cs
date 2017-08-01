using ESH.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Examples
{
    public class LogFileExample
    {
        public static void Test()
        {
            // Always wrap the log file in a using statement 
            using (var logFile = new LogFile(logToFile: true, logToConsole: true))
            {
                // The log file should show up under the same folder as the executing code, in the Log folder
                Console.WriteLine("Log Path: " + logFile.LogPath);
                // The tag prefix for the created log files
                Console.WriteLine("Log Tag: " + logFile.LogTag);
                logFile.WriteLine("Log file test");

            }
        }
    }
}
