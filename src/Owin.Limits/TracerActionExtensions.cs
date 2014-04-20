namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    internal static class TracerActionExtensions
    {
        internal static void AsVerbose(this Tracer source, string message)
        {
            source(TraceEventType.Verbose, message);
        }

        internal static void AsVerbose(this Tracer source, string message, params object[] args)
        {
            source(TraceEventType.Verbose, message.FormatWith(args));
        }

        internal static void AsInfo(this Tracer source, string message)
        {
            source(TraceEventType.Information, message);
        }

        internal static void AsInfo(this Tracer source, string message, params object[] args)
        {
            source(TraceEventType.Information, message.FormatWith(args));
        }
    }
}