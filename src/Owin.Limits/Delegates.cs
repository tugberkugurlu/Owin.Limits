namespace Owin.Limits
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate that represents a trace operation.
    /// </summary>
    /// <param name="traceEventType">Identifies the type of event that has caused the trace.</param>
    /// <param name="message">The trace message.</param>
    public delegate void Tracer(TraceEventType traceEventType, string message);

    /// <summary>
    /// Owin application func.
    /// </summary>
    /// <param name="environment"></param>
    internal delegate Task AppFunc(IDictionary<string, object> environment);
}