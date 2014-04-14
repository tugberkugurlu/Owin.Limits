namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    internal static class DefaultDelegateHelper
    {
        public static Action<TraceEventType, string> Tracer = (type, msg) => { };
        public static Func<int, string> ReasonPhrase = code => "";
    }
}