namespace Owin.Limits
{
    using System;
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

    internal static class EnviromentExtensions
    {
        internal static AppFunc ToAppFunc(this Func<IDictionary<string, object>, Task> appFunc)
        {
            appFunc.MustNotNull("appFunc");
            return env => appFunc(env);
        }
    }
}