namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    internal static class DefaultDelegateHelper
    {
        public static readonly Action<TraceEventType, string> Tracer = (type, msg) => { };
        public static readonly Func<int, string> ReasonPhrase = code => string.Empty;
    }
}