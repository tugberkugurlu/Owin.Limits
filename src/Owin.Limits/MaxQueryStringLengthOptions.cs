﻿namespace Owin.Limits
{
    using System;

    /// <summary>
    /// Options for limiting the querstring length.
    /// </summary>
    public class MaxQueryStringLengthOptions : OptionsBase
    {
        private Func<int, string> _limitReachedReasonPhrase;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxQueryStringLengthOptions"/> class.
        /// </summary>
        /// <param name="maxQueryStringLength">Maximum length of the query string.</param>
        public MaxQueryStringLengthOptions(int maxQueryStringLength)
            : this(() => maxQueryStringLength)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxQueryStringLengthOptions"/> class.
        /// </summary>
        /// <param name="getMaxQueryStringLength">A delegate to get the maximum query string length.</param>
        public MaxQueryStringLengthOptions(Func<int> getMaxQueryStringLength)
        {
            GetMaxQueryStringLength = getMaxQueryStringLength;
        }

        internal Func<int> GetMaxQueryStringLength { get; private set; }

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