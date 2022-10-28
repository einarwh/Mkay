using System;
using System.Text.RegularExpressions;

namespace Mkay
{
    public static class MethodLibrary
    {
        public static int Length(string s)
        {
            return s == null ? 0 : s.Length;
        }

        public static bool IsMatch(string input, string pattern)
        {
            return new Regex(pattern).IsMatch(input);
        }

        public static string Reverse(string s) 
        {
            var array = s.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        public static string Cut(string input, int startIndex, int endIndex)
        {
            Func<int, int> f = ix => (ix + input.Length) % input.Length;
            var start = f(startIndex);
            var end = f(endIndex);
            var len = end - start;
            var result = input.Substring(start, len);
            return result;
        }
    }
}