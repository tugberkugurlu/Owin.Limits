namespace Owin.Limits
{
    using System;

    /// <summary>
    /// Options to limit the length of an URL.
    /// </summary>
    public class MaxUrlLengthOptions : OptionsBase
    {
        private Func<int, string> _limitReachedReasonPhrase;

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