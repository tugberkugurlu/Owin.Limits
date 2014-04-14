namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Options for a timeout connection.
    /// </summary>
    public class ConnectionTimeoutOptions
    {
        private Action<TraceEventType, string> _trace;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionTimeoutOptions"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public ConnectionTimeoutOptions(TimeSpan timeout) : this(() => timeout)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionTimeoutOptions"/> class.
        /// </summary>
        /// <param name="getTimeout">A delegate to retrieve the timeout timespan. Allows you
        /// to supply different values at runtime.</param>
        public ConnectionTimeoutOptions(Func<TimeSpan> getTimeout)
        {
            GetTimeout = getTimeout;
        }

        internal Func<TimeSpan> GetTimeout { get; private set; }

        /// <summary>
        /// Gets or sets the delegate to trace the middleware.
        /// </summary>
        public Action<TraceEventType, string> Tracer
        {
            get { return _trace ?? DefaultDelegateHelper.Tracer; }
            set { _trace = value; }
        }
    }
}