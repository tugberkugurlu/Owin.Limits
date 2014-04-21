namespace Owin.Limits
{
    using System.Diagnostics;

    /// <summary>
    /// Delegate that represents a trace operation.
    /// </summary>
    /// <param name="traceEventType">Identifies the type of event that has caused the trace.</param>
    /// <param name="message">The trace message.</param>
    public delegate void Tracer(TraceEventType traceEventType, string message);
}