using System;

namespace PryRutasMoviles.Helpers
{
    public static class Util
    {
        public static long GetCurrentDateTime()
        {

            return Convert.ToInt64($"{DateTime.Now.Year}" +
                $"{DateTime.Now.Month.ToString().PadLeft(2, '0')}" +
                $"{DateTime.Now.Day.ToString().PadLeft(2, '0')}" +
                $"{DateTime.Now.Hour.ToString().PadLeft(2, '0')}" +
                $"{DateTime.Now.Minute.ToString().PadLeft(2, '0')}");
        }

        public static long SetMeetingDate(string meetingTime)
        {
            var meetingTimeSplit = meetingTime.Split(':');
            return Convert.ToInt64($"{DateTime.Now.Year}" +
                $"{DateTime.Now.Month.ToString().PadLeft(2, '0')}" +
                $"{DateTime.Now.Day.ToString().PadLeft(2, '0')}" +
                $"{meetingTimeSplit[0]}" +
                $"{meetingTimeSplit[1]}");
        }

        public static string FormatMeetingDate(string meetingTime) 
        {
            var meetingTimeSplit = meetingTime.Split(':');
            return $"{DateTime.Now.Year}/" +
                $"{DateTime.Now.Month.ToString().PadLeft(2, '0')}/" +
                $"{DateTime.Now.Day.ToString().PadLeft(2, '0')} " +
                $"{meetingTimeSplit[0]}:" +
                $"{meetingTimeSplit[1]}";
        }

    }
}
