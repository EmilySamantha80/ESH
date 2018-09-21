// Title:  ICS calendar item creation
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

namespace ESH.Utility
{
    public static class Email
    {
        /// <summary>
        /// Creates an ICS calendar item
        /// </summary>
        /// <param name="startDate">Calendar item Start Date/Time</param>
        /// <param name="endDate">Calendar item End Date/Time</param>
        /// <param name="summary">Calendar item summary</param>
        /// <param name="location">Calendar item location</param>
        /// <param name="description">Calendar item description</param>
        /// <param name="notificationTime">Notification time examples: "-PT30M" (30 minutes), "-PT1H" (1 hour), "-P1D" (1 day), "-P1DT12H" (1.5 days). See https://www.kanzaki.com/docs/ical/duration-t.html</param>
        /// <returns>String containing the ICS calendar item</returns>
        public static string CreateICS(DateTime startDate, DateTime endDate, string summary, string location, string description, string notificationTime)
        {
            string ics = "";
            ics += "BEGIN:VCALENDAR" + Environment.NewLine;
            ics += "VERSION:2.0" + Environment.NewLine;
            ics += "PRODID:-//hacksw/handcal//NONSGML v1.0//EN" + Environment.NewLine;
            ics += "BEGIN:VEVENT" + Environment.NewLine;
            ics += "UID:" + Guid.NewGuid() + Environment.NewLine;
            ics += "DSTAMP;TZID=/US/Pacific:" + ConvertToICSDateTime(DateTime.Now) + Environment.NewLine;
            ics += "DTSTART;TZID=/US/Pacific:" + ConvertToICSDateTime(startDate) + Environment.NewLine;
            ics += "DTEND;TZID=/US/Pacific:" + ConvertToICSDateTime(endDate) + Environment.NewLine;
            ics += "SUMMARY:" + summary + Environment.NewLine;
            ics += "LOCATION:" + location + Environment.NewLine;
            ics += "DESCRIPTION:" + description.Replace(Environment.NewLine, @"\n") + Environment.NewLine;
            if (notificationTime != null)
            {
                ics += "BEGIN:VALARM" + Environment.NewLine;
                ics += "DESCRIPTION:REMINDER" + Environment.NewLine;
                //Notification time examples: "-PT30M" (30 minutes), "-PT1H" (1 hour), "-P1D" (1 day), "-P1DT12H" (1.5 days)
                //https://www.kanzaki.com/docs/ical/duration-t.html
                ics += "TRIGGER:" + notificationTime + Environment.NewLine;
                ics += "ACTION:DISPLAY" + Environment.NewLine;
                ics += "END:VALARM" + Environment.NewLine;
            }
            ics += "END:VEVENT" + Environment.NewLine;
            ics += "END:VCALENDAR" + Environment.NewLine;

            return ics;
        }

        /// <summary>
        /// Converts a DateTime object into an ICS date/time string
        /// </summary>
        /// <param name="input">DateTime object to convert</param>
        /// <returns>ICS date/time string</returns>
        public static string ConvertToICSDateTime(DateTime input)
        {
            return input.ToString("yyyyMMddTHmmss");
        }
    }
}