namespace Owin.Limits
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Options for limitng the request content length.
    /// </summary>
    public class MaxRequestContentLengthOptions
    {
        private Func<int, string> _limitReachedReasonPhrase;
        private Action<TraceEventType, string> _trace;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxRequestContentLengthOptions"/> class.
        /// </summary>
        /// <param name="maxContentLength">Maximum length of the content.</param>
        public MaxRequestContentLengthOptions(int maxContentLength) : this(() => maxContentLength)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxRequestContentLengthOptions"/> class.
        /// </summary>
        /// <param name="getMaxContentLength">A delegate to get the maximum content length.</param>
        public MaxRequestContentLengthOptions(Func<int> getMaxContentLength)
        {
            GetMaxContentLength = getMaxContentLength;
        }

        internal Func<int> GetMaxContentLength { get; set; }

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