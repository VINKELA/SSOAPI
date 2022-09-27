using PowerTrackEnterprise.EnumLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utility.Logging;

namespace PowerTrackEnterprise.Core.Utility
{
    public class SharedUtilities
    {
        public static IEnumerable<SplitDateTime> SplitDateRange(DateTime start, DateTime end)
        {
            var returnData = new List<SplitDateTime>();
            try
            {
                while (start <= end)
                {
                    returnData.Add(new SplitDateTime(startDateTime: start, endDateTime: start.AddDays(1).AddSeconds(-1)));
                    start = start.AddDays(1);
                }
            }
            catch (Exception e)
            {
                ActivityLogger.Log(e);
            }
            return returnData;
        }
        public static string CapitaliseFirstLetterInAString(string text)
        {
            string firstLetterOfEachWord = null;
            if (text.Split(' ').Length == 1) firstLetterOfEachWord = text.Substring(0, 1).ToUpper() + text.Substring(1);
            else
            {
                firstLetterOfEachWord =
                       string.Join(" ", text.ToLower().Split(' ').ToList()
                               .ConvertAll(word =>
                                       word.Substring(0, 1).ToUpper() + word.Substring(1)
                               )
                       );
            }
            return firstLetterOfEachWord;
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static string SplitString(string word)
        {
            string result = null;
            if (word.Length > 0)
            {
                int i = word.IndexOf(" ") + 1;
                string str = word.Substring(i);
                result = str;
            }
            return result;
        }
    }

    public static class IsDigits
    {
        public static bool IsDigitsOnly(string scannedText)
        {
            var ans = scannedText.All(char.IsDigit);
            return ans;
        }
    }

    public class SplitDateTime
    {
        public SplitDateTime(DateTime startDateTime, DateTime endDateTime)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }

    public static class IntegerExtensions
    {
        public static int ParseInt(this string value, int defaultIntValue = 0)
        {
            int parsedInt;
            return int.TryParse(value, out parsedInt) ? parsedInt : defaultIntValue;
        }

        public static long ParseLong(this string value, long defaultlongValue = 0)
        {
            long parsedInt;
            return long.TryParse(value, out parsedInt) ? parsedInt : defaultlongValue;
        }

        public static double ParseDouble(this string value, double defaultDoubleValue = 0)
        {
            double parsedDouble;
            return Double.TryParse(value, out parsedDouble) ? parsedDouble : defaultDoubleValue;

        }
        public static int? ParseNullableInt(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return value.ParseInt();
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");

            var indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
        public static class PasswordStrength
        {
            public static PasswordScore CheckPasswordStrength(string password)
            {
                int score = 1;

                if (password.Length < 1)
                    return PasswordScore.Blank;
                if (password.Length < 4)
                    return PasswordScore.VeryWeak;

                if (password.Length >= 7)
                    score++;
                if (password.Length >= 8)
                    score++;
                if (Regex.Match(password, @"\d+", RegexOptions.ECMAScript).Success)
                    score++;
                if (Regex.Match(password, @"[a-z]").Success && Regex.Match(password, @"/[A-Z]/").Success)
                    score++;
                if (Regex.Match(password, @".[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript).Success)
                    score++;

                return (PasswordScore)score;
            }
        }


    }

    public static class GenerateRandomString
    {
        public static string RandomString(int length = 12)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            const string special = "!@#$%^&*";

            var middle = length / 2;
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                if (middle == length)
                {
                    res.Append(number[rnd.Next(number.Length)]);
                }
                else if (middle - 1 == length)
                {
                    res.Append(special[rnd.Next(special.Length)]);
                }
                else
                {
                    if (length % 2 == 0)
                    {
                        res.Append(lower[rnd.Next(lower.Length)]);
                    }
                    else
                    {
                        res.Append(upper[rnd.Next(upper.Length)]);
                    }
                }
            }
            return res.ToString();
        }

        public static string TransactionReferenceCode(string transactionReference)
        {
            var code = transactionReference.Split('-');
            return code[0] + '-' + code[1];

        }

    }
}
