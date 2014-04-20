namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    internal static class TracerActionExtensions
    {
        internal static void AsVerbose(this Action<TraceEventType, string> source, string message)
        {
            source(TraceEventType.Verbose, message);
        }

        internal static void AsInfo(this Action<TraceEventType, string> source, string message)
        {
            source(TraceEventType.Information, message);
        }
    }
}