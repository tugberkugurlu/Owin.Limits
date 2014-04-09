namespace Owin.Limits {
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Options for limiting the max bandwidth.
    /// </summary>
    public class MaxBandwidthOptions {
        private Action<TraceEventType, string> _trace;

        internal Func<int> GetMaxBytesPerSecond { get; set; }
        /// <summary>
        /// Gets or sets the delegate to trace the middleware.
        /// </summary>
        public Action<TraceEventType, string> Tracer {
            get { return _trace ?? DefaultHelper.Tracer; }
            set { _trace = value; }
        }

        public MaxBandwidthOptions(int maxBytesPerSecond) : this(() => maxBytesPerSecond) {
        }
        public MaxBandwidthOptions(Func<int> getMaxBytesPerSecond) {
            GetMaxBytesPerSecond = getMaxBytesPerSecond;
        }
    }
}