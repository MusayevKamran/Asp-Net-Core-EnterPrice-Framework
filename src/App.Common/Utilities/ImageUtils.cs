using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace App.Common.Utilities
{
    public static class ImageUtils
    {
        public static Guid GetImageName(string imagePath)
        {
            string pattern = @"([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})";
            var gid = Regex.Matches(imagePath, pattern).Cast<Match>().Select(m => m.Value).First();

            return Guid.Parse(gid);
        }
    }
}
