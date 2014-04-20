namespace Owin.Limits
{
    using System.Diagnostics;

    internal static class TracerActionExtensions
    {
        internal static void AsVerbose(this Tracer source, string message)
        {
            source(TraceEventType.Verbose, message);
        }

        internal static void AsInfo(this Tracer source, string message)
        {
            source(TraceEventType.Information, message);
        }
    }
}