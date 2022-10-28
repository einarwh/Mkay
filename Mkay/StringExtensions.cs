namespace Mkay
{
    public static class StringExtensions
    {
        public static string With(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
    }
}
