using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace App.Common.Utilities
{
    public static class EmailUtils
    {

        public static readonly Regex EmailValidationRegex = new Regex(EmailValidationPattern, RegexOptions.Compiled);
        public const string EmailValidationPattern = @"^([\w!#$%&'*\-/=?\^_`{|}~]+(\.[\w!#$%&'*\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{1,64})|(([0-9]{1,3}\.){3}[0-9]{1,3})))?$";

        public static string MaskEmail(string email)
        {

            string _PATTERN = @"(?<=^.)[^@]*|(?<=@).*(?=.\.[^.]+$)";
            return Regex.Replace(email, _PATTERN, m => new string('*', m.Length));
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailValidationRegex.IsMatch(email.Trim().ToLower());
        }
    }
}

