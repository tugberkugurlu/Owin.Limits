namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Options for limiting the max bandwidth.
    /// </summary>
    public class MaxBandwidthOptions
    {
        private Action<TraceEventType, string> _trace;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxBandwidthOptions"/> class.
        /// </summary>
        /// <param name="maxBytesPerSecond">The maximum number of bytes per second to be transferred. Use 0 or a negative
        /// number to specify infinite bandwidth.</param>
        public MaxBandwidthOptions(int maxBytesPerSecond) : this(() => maxBytesPerSecond)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxBandwidthOptions"/> class.
        /// </summary>
        /// <param name="getMaxBytesPerSecond">A delegate to retrieve the maximum number of bytes per second to be transferred.
        /// Allows you to supply different values at runtime. Use 0 or a negative number to specify infinite bandwidth.</param>
        public MaxBandwidthOptions(Func<int> getMaxBytesPerSecond)
        {
            GetMaxBytesPerSecond = getMaxBytesPerSecond;
        }

        internal Func<int> GetMaxBytesPerSecond { get; private set; }

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