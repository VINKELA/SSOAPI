using System;
using System.Linq;

namespace SSOService.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string word)
        {
            if (string.IsNullOrEmpty(word)) return word;
            var afterSpace = true;
            var result = "";
            var n = word.Length;
            var spaceCharacters = new char[] { '\n', '\t', ' ' };
            for (int i = 0; i < n; i++)
            {
                var currentCharacter = word[i];
                if (afterSpace)
                    result += currentCharacter.ToString().ToUpper();
                else
                    result += currentCharacter.ToString().ToLower();
                afterSpace = spaceCharacters.Contains(currentCharacter);
            }
            return result;
        }

        public static bool IsValidEmailFormat(this string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
