namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Options for limiting the number of concurrent requests.
    /// </summary>
    public class MaxConcurrentRequestOptions
    {
        private Func<int, string> _limitReachedReasonPhrase;
        private Action<TraceEventType, string> _trace;


        /// <summary>
        /// Initializes a new instance of the <see cref="MaxConcurrentRequestOptions"/> class.
        /// </summary>
        /// <param name="maxConcurrentRequests">The maximum number of concurrent requests. Use 0 or a negative
        /// number to specify unlimited number of concurrent requests.</param>
        public MaxConcurrentRequestOptions(int maxConcurrentRequests) : this(() => maxConcurrentRequests)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxConcurrentRequestOptions"/> class.
        /// </summary>
        /// <param name="getMaxConcurrentRequests">A delegate to retrieve the maximum number of concurrent requests. Allows you
        /// to supply different values at runtime. Use 0 or a negative number to specify unlimited number of concurrent requests.</param>
        public MaxConcurrentRequestOptions(Func<int> getMaxConcurrentRequests)
        {
            GetMaxConcurrentRequests = getMaxConcurrentRequests;
        }

        internal Func<int> GetMaxConcurrentRequests { get; set; }

        /// <summary>
        /// Gets or sets the delegate to trace the middleware.
        /// </summary>
        public Action<TraceEventType, string> Tracer
        {
            get { return _trace ?? DefaultDelegateHelper.Tracer; }
            set { _trace = value; }
        }

        /// <summary>
        /// Gets or sets the delegate to set a reasonphrase.<br/>
        /// Default reasonphrase is empty.
        /// </summary>
        public Func<int, string> LimitReachedReasonPhrase
        {
            get { return _limitReachedReasonPhrase ?? DefaultDelegateHelper.ReasonPhrase; }
            set { _limitReachedReasonPhrase = value; }
        }
    }
}