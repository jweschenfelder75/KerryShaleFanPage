using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace KerryShaleFanPage.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string ToQueryString(this NameValueCollection? nvc)
        {
            if (nvc == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var key in nvc.Keys)
            {
                var stringKey = key as string ?? string.Empty;
                if (string.IsNullOrWhiteSpace(stringKey))
                {
                    continue;
                }

                var values = nvc.GetValues(stringKey);
                if (values == null)
                {
                    continue;
                }

                foreach (var value in values)
                {
                    sb.Append(sb.Length == 0 ? "?" : "&");
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(stringKey), Uri.EscapeDataString(value));
                }
            }

            return sb.ToString();
        }

        public static string ComputeMd5(this string str)
        {
            var sb = new StringBuilder();

            using (var md5 = MD5.Create())
            {
                var hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

                foreach (var b in hashValue)
                {
                    sb.Append($"{b:X2}");
                }
            }

            return sb.ToString();
        }

        public static DateTime? ToDateTime(this string? dateTimeAsStr, string? formatStr, DateTime? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(dateTimeAsStr) || string.IsNullOrWhiteSpace(formatStr))
            {
                return fallbackValue;
            }

            return DateTime.TryParseExact(dateTimeAsStr, formatStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) 
                ? result 
                : fallbackValue;
        }
    }
}
