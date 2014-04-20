namespace Owin.Limits
{
    using System;

    internal static class DefaultDelegateHelper
    {
        public static readonly Tracer DefaultTracer = (type, msg) => { };
        public static readonly Func<int, string> DefaultReasonPhrase = _ => string.Empty;
    }
}