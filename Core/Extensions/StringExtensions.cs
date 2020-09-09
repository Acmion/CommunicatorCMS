using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCmsLibrary.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string text, string oldValue, string newValue)
        {
            var index = text.IndexOf(oldValue);

            if (index < 0)
            {
                return text;
            }

            return text.Substring(0, index) + newValue + text.Substring(index + oldValue.Length);
        }

        public static string FirstLetterToUpperCase(this string s)
        {
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}
