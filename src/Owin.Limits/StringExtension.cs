// ReSharper disable once CheckNamespace
namespace System
{
    internal static class StringExtension
    {
        internal static string FormatWith(this string source, params object[] args)
        {
            return string.Format(source, args);
        }
    }
}