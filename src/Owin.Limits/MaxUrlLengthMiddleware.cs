namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Owin;

    internal class MaxUrlLengthMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly MaxUrlLengthOptions _options;

        public MaxUrlLengthMiddleware(Func<IDictionary<string, object>, Task> next, MaxUrlLengthOptions options)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            _options.Tracer.AsVerbose("Start processing.");

            var context = new OwinContext(environment);
            int maxUrlLength = _options.GetMaxUrlLength();
            string unescapedUri = Uri.UnescapeDataString(context.Request.Uri.AbsoluteUri);

            _options.Tracer.AsVerbose("Checking request url length.");
            if (unescapedUri.Length > maxUrlLength)
            {
                _options.Tracer.AsInfo("Url \"{0}\"(Length: {2}) exceeds allowed length of {1}. Request rejected.".FormattedWith(unescapedUri, maxUrlLength,
                    unescapedUri.Length));
                context.Response.StatusCode = 414;
                context.Response.ReasonPhrase = _options.LimitReachedReasonPhrase(context.Response.StatusCode);
                return;
            }
            _options.Tracer.AsVerbose("Check passed. Request forwarded.");
            await _next(environment);
            _options.Tracer.AsVerbose("Processing finished.");
        }
    }

    /// <summary>
    /// Options to limit the length of an URL.
    /// </summary>
    public class MaxUrlLengthOptions
    {
        private Func<int, string> _limitReachedReasonPhrase;
        private Action<TraceEventType, string> _trace;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxUrlLengthOptions"/> class.
        /// </summary>
        /// <param name="maxUrlLength">Maximum length of the URL.</param>
        public MaxUrlLengthOptions(int maxUrlLength) : this(() => maxUrlLength)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxUrlLengthOptions"/> class.
        /// </summary>
        /// <param name="getMaxUrlLength">A delegate to get the maximum URL length.</param>
        public MaxUrlLengthOptions(Func<int> getMaxUrlLength)
        {
            GetMaxUrlLength = getMaxUrlLength;
        }

        internal Func<int> GetMaxUrlLength { get; set; }

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