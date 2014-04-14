namespace Owin.Limits
{
    internal static class StringExtension
    {
        public static string FormattedWith(this string source, params object[] args)
        {
            return string.Format(source, args);
        }
    }
}