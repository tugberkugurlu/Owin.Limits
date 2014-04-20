namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    internal static class TracerActionExtensions
    {
        public static void AsVerbose(this Action<TraceEventType, string> source, string message)
        {
            source(TraceEventType.Verbose, message);
        }

        public static void AsInfo(this Action<TraceEventType, string> source, string message)
        {
            source(TraceEventType.Information, message);
        }
    }
}