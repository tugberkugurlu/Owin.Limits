namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Base options class
    /// </summary>
    public abstract class OptionsBase
    {
        private Action<TraceEventType, string> _tracer;

        /// <summary>
        /// Gets or sets the delegate to trace the middleware.
        /// </summary>
        public Action<TraceEventType, string> Tracer
        {
            get { return _tracer ?? DefaultDelegateHelper.Tracer; }
            set { _tracer = value; }
        }
    }
}