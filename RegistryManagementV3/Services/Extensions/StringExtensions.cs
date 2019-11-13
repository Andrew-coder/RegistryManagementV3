using System;
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
    }
}