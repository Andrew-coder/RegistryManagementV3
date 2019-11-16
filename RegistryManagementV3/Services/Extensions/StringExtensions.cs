using System;
using System.Globalization;
using System.IO;

namespace RegistryManagementV3.Services.Extensions
{
    public static class StringExtensions
    {
        public static string AppendTimeStamp(this string fileName)
        {
            return string.Concat(
                Path.GetFileNameWithoutExtension(fileName),
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(fileName)
            );
        }
        
        public static string RemoveTimeStamp(this string fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            return name.Substring(0, name.Length - 17) + extension;
        }

        public static Tuple<DateTime, DateTime> ParseToDateRange(this string dateRange)
        {
            var dates = dateRange?.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if(dates != null && dates.Length == 2) {
                const string format = "MM/dd/yyyy";
                var provider = CultureInfo.InvariantCulture;
                var startDate = DateTime.ParseExact(dates[0].Trim(), format, provider);
                var endDate = DateTime.ParseExact(dates[1].Trim(), format, provider);
                return new Tuple<DateTime, DateTime>(startDate, endDate);
            }
            else
            {
                return null;
            }         
        }
    }
}