namespace Owin.Limits
{
    using System;

    /// <summary>
    /// Options for limitng the request content length.
    /// </summary>
    public class MaxRequestContentLengthOptions : OptionsBase
    {
        private Func<int, string> _limitReachedReasonPhrase;

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

        internal Func<int> GetMaxContentLength { get; private set; }

        /// <summary>
        /// Gets or sets the delegate to set a reasonphrase.<br/>
        /// Default reasonphrase is empty.
        /// </summary>
        public Func<int, string> LimitReachedReasonPhrase
        {
            get { return _limitReachedReasonPhrase ?? DefaultDelegateHelper.DefaultReasonPhrase; }
            set { _limitReachedReasonPhrase = value; }
        }
    }
}