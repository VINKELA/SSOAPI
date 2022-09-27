using PowerTrackEnterprise.EnumLibrary.Enums;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Utility.Logging;

namespace PowerTrackEnterprise.Core.Utility
{
    public static class Transforms
    {
        public static string NormalizePhoneNumber(string rawPhoneNumber)
        {
            if (string.IsNullOrEmpty(rawPhoneNumber))
                return "00000000000";

            if (rawPhoneNumber.Length == 10 && rawPhoneNumber[0] != '0')
                return FormatPhoneNumber("0" + rawPhoneNumber);
            else return FormatPhoneNumber(rawPhoneNumber);
        }

        public static string TrimSpacesBetweenString(string rawString)
        {
            var mystring = rawString.Split(' ');
            var result = mystring.Select(mstr => mstr.Trim()).Where(ss => !string.IsNullOrEmpty(ss))
                .Aggregate(string.Empty, (current, ss) => current + ss + " ");
            return result.Trim();
        }

        public static List<List<T>> ListChunk<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return "2340000000000";

            if (phoneNumber.Length == 10 && phoneNumber[0] != '0')
                phoneNumber = "234" + phoneNumber;
            if (phoneNumber.Length == 11 && phoneNumber[0] == '0')
                phoneNumber = "234" + phoneNumber.Substring(1);

            return phoneNumber;
        }

        public static DateTime? ProcessDateUpperBound(DateTime lastProcessDate, RecurrenceType recurrenceInterval, out DateTime lowerBound)
        {
            var date = DateTime.Now;
            lowerBound = date;

            try
            {
                switch (recurrenceInterval)
                {
                    case RecurrenceType.Day:
                        if (DateTime.Now.Date.Subtract(lastProcessDate.Date).TotalDays > 0) // current day - last processed is greater than 0, we're at least one day ahead.
                        {
                            lowerBound = lowerBound.AddDays(-1);
                            return DateTime.Now.Date; // if upper  bound  is greater than today, how are we going to get reports
                        }
                        break;
                    case RecurrenceType.Month:
                        if (DateTime.Now.Month != lastProcessDate.Month)
                        {
                            //lowerBound = lowerBound.AddMonths(-1);
                            lowerBound = lastProcessDate; // from last processed date 
                            //return new DateTime(lastProcessDate.Year, lastProcessDate.Month, 1).Date;
                            return new DateTime(date.Year, date.Month, 1).Date; // to first day of current month
                        }
                        break;
                }
                return null;
            }
            catch (Exception e)
            {
                ActivityLogger.Log(e);
                return null;
            }
        }

        public static bool ParseNumber(string value)
        {
            try
            {
                long number;
                var res = Int64.TryParse(value, out number);
                return res;
            }
            catch (Exception e)
            {
                ActivityLogger.Log(e);
                return false;
            }
        }

        public static string CapitalizeFirstLetter(string data)
        {
            var res = data != null ? char.ToUpper(data[0]) + data.Substring(1) : data;
            return res;
        }
        public static double ConvertKWToKWH(double valueInKW, int integrationPeriod = 0)
        {
            try
            {
                if (integrationPeriod == 0)
                {
                    integrationPeriod = ConfigurationManager.AppSettings["IntegrationPeriod"].ToInt();
                }
                return (valueInKW * integrationPeriod) / 60;

            }
            catch (Exception e)
            {
                ActivityLogger.Log(e);
                return 0.0;
            }
        }
    }
}
