using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;

namespace App.Common.Utilities
{
    public static class PhoneUtils
    {
        public static string MaskPhoneNumber(string phoneNumber)
        {
            var firstDigits = phoneNumber.Substring(0, 4);
            var lastDigits = phoneNumber.Substring(phoneNumber.Length - 2, 2);

            var requiredMask = new String('*', phoneNumber.Length - firstDigits.Length - lastDigits.Length);

            var maskedString = string.Concat(firstDigits, requiredMask, lastDigits);
            string masekedPhone = Regex.Replace(maskedString, ".{4}", "$0");
            return masekedPhone;

        }
    }
}

