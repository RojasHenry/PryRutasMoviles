using System;
using System.Collections.Generic;
using System.Text;

namespace PryRutasMoviles.Helpers
{
    public static class Util
    {
        public static long GetCurrentDateTime()
        {
            return Convert.ToInt64($"{DateTime.Now.Year}" +
                $"{DateTime.Now.Month}" +
                $"{DateTime.Now.Day}" +
                $"{DateTime.Now.Hour}" +
                $"{DateTime.Now.Minute}" +
                $"{DateTime.Now.Second}");
        }
        public static long SetMeetingDate(string meetingTime)
        {
            return Convert.ToInt64($"{DateTime.Now.Year}" +
                $"{DateTime.Now.Month}" +
                $"{DateTime.Now.Day}" +
                $"{meetingTime.Replace(":", string.Empty)}");
        }

        public static string FormatMeetingDate(string meetingTime) 
        {
            return $"{DateTime.Now.Year}/" +
                $"{DateTime.Now.Month}/" +
                $"{DateTime.Now.Day} " +
                $"{meetingTime}";
        }

    }
}
