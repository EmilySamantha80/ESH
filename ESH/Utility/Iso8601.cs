// Title:  ISO 8601 Date/Time and Week Date utility class
// Author: Emily Heiner
// Date:   2016-08-04
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
using System.Globalization;
using NodaTime; // NodaTime is used for Time Zone conversion, and can be downloaded at http://nodatime.org or using NuGet.

namespace ESH.Utility
{
    public class Iso8601
    {
        /// <summary>
        /// Specifies an ISO 8601 day of the week
        /// </summary>
        public enum DayOfWeek
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }

        /// <summary>
        /// Get an ISO 8601 Date/Time string from a Gregorian Calendar date. Note: This function relies on NodaTime, which can be downloaded at http://nodatime.org
        /// <param name="date">Gregorian Calendar Date to generate the ISO 8601 Date/Time string from</param>
        /// <param name="displayAsZulu">Convert the date/time to UTC and add 'Z' to the string. Note: If this is true, the 'convertToUtc' parameter and 'showTimeZone' parameter will be ignored.</param>
        /// <param name="convertToUtc">Convert the date/time to UTC before creating the string</param>
        /// <param name="showTimeZone">Add the time zone to the string in +HH:MM/-HH:MM format.</param>
        /// <param name="dtz">NodaTime.DateTimeZone for the input DateTime object</param> 
        /// </summary>
        public static string ToIso8601String(DateTime input, bool displayAsZulu = false, bool convertToUtc = false, bool showTimeZone = false, NodaTime.DateTimeZone dtz = null)
        {
            // Set our ISO 8601 Date/Time format string
            string iso8601format = "yyyy-MM-ddTHH:mm:ss";
            // The time zone specifier to be added to the ISO 8601 Date/Time string, if requested.
            string timeZoneSpecifier = string.Empty;
            // The output Date/Time value (will be adjusted for UTC if necessary)
            DateTime output;

            // Get the NodaTime Date/Time Zone Provider
            NodaTime.IDateTimeZoneProvider timeZoneProvider = NodaTime.DateTimeZoneProviders.Tzdb;
            if (dtz == null)
            {
                // If the NodaTime Date/Time Zone wasn't specified, use the system default
                dtz = timeZoneProvider.GetSystemDefault();
            }

            // Create a NodaTime Zoned Date/Time object from the input Date/Time
            NodaTime.ZonedDateTime zonedDateTime = dtz.AtLeniently(NodaTime.LocalDateTime.FromDateTime(input));

            //Convert to UTC if necessary
            if (convertToUtc || displayAsZulu)
                output = zonedDateTime.ToDateTimeUtc();
            else
                output = zonedDateTime.ToDateTimeUnspecified();

            if (displayAsZulu)
            {
                // If we're displaying as Zulu, add a "Z" to the end of the ISO 8601 Date/Time string
                timeZoneSpecifier = "Z";
            }
            else if (showTimeZone)
            {
                if (convertToUtc)
                {
                    // If we are outputting as UTC, we know the time zone offset
                    timeZoneSpecifier = "+00:00";
                }
                else
                {
                    // Set the time zone specifier to the proper offset
                    timeZoneSpecifier = zonedDateTime.Offset.ToString("+HH:mm", CultureInfo.InvariantCulture);
                }
            }

            // Return our final ISO 8601 Date/Time representation
            return string.Concat(output.ToString(iso8601format), timeZoneSpecifier);
        }

        /// <summary>
        /// ISO 8601 Week Date representation of a Gregorian Calendar Date
        /// Adapted from: https://blogs.msdn.microsoft.com/shawnste/2006/01/24/iso-8601-week-of-year-format-in-microsoft-net/
        /// </summary>
        public class WeekDate
        {
            /// <summary>
            /// Gregorian Calendar Date
            /// </summary>
            private readonly DateTime _Date;
            /// <summary>
            /// Gregorian Calendar Date
            /// </summary>
            public DateTime Date
            {
                get { return this._Date; }
            }

            /// <summary>
            /// ISO 8601 Week Date 'Year' part
            /// </summary>
            private readonly int _Year;
            /// <summary>
            /// ISO 8601 Week Date 'Year' part
            /// </summary>
            public int Year
            {
                get { return this._Year; }
            }

            /// <summary>
            /// ISO 8601 Week Date 'Week' part
            /// </summary>
            private readonly int _Week;
            /// <summary>
            /// ISO 8601 Week Date 'Week' part
            /// </summary>
            public int Week
            {
                get { return this._Week; }
            }

            /// <summary>
            /// ISO 8601 Week Date 'Day' part
            /// </summary>
            private readonly int _Day;
            /// <summary>
            /// ISO 8601 Week Date 'Day' part
            /// </summary>
            public int Day
            {
                get { return this._Day; }
            }

            public DayOfWeek DayOfWeek
            {
                get
                {
                    return (DayOfWeek)this.Day;
                }
            }

            /// <summary>
            /// ISO 8601 Week Date string representation
            /// </summary>
            private readonly string _WeekDate;

            /// <summary>
            /// Get an ISO 8601 Week Date from a Gregorian Calendar Date
            /// <param name="date">Gregorian Calendar Date to generate the ISO 8601 Week Date from</param>
            /// </summary>
            public WeekDate(DateTime date)
            {
                //We only care about the date, not the time
                this._Date = date.Date;

                // Get the ISO 8601 Week Number for the given year
                this._Week = GetIso8601WeekOfYear(this.Date);

                // Compensate for ISO 8601 Week Date Year part being a different year in some instances.
                if (this.Week == 53 && this.Date.Month == 1)
                    this._Year = this.Date.Year - 1;
                else if (this.Week == 1 && this.Date.Month == 12)
                    this._Year = this.Date.Year + 1;
                else
                    this._Year = date.Year;

                // Get the day of the week, starting with Monday
                this._Day = (((int)this.Date.DayOfWeek + 6) % 7) + 1;

                // Set the week date in the ISO 8601 Week Date Format
                this._WeekDate = string.Format("{0:0000}-W{1:00}-{2}", this.Year, this.Week, this.Day);
            }

            /// <summary>
            /// Get an ISO 8601 'Week' part from a Gregorian Calendar Date
            /// <param name="date">Gregorian Calendar Date to generate the ISO 8601 'Week' part from</param>
            /// </summary>
            public static int GetIso8601WeekOfYear(DateTime date)
            {
                // Need a calendar.  Culture’s irrelevent since we specify start day of week
                Calendar cal = CultureInfo.InvariantCulture.Calendar;

                // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it’ll 
                // be the same week# as whatever Thursday, Friday or Saturday are,
                // and we always get those right
                System.DayOfWeek day = cal.GetDayOfWeek(date);
                if (day >= System.DayOfWeek.Monday && day <= System.DayOfWeek.Wednesday)
                {
                    date = date.AddDays(3);
                }

                // Return the week of our adjusted day
                return cal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Monday);
            }

            /// <summary>
            /// Returns a DateTime object representing the first day of the week
            /// </summary>
            public DateTime FirstDayOfWeek()
            {
                return this.Date.AddDays((-1 * this.Day) + 1);
            }

            /// <summary>
            /// Returns the ISO 8601 Week Date string representation
            /// </summary>
            public override string ToString()
            {
                return this._WeekDate;
            }
        }
    }
}