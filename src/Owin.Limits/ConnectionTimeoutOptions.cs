namespace Owin.Limits
{
    using System;

    /// <summary>
    /// Options for a timeout connection.
    /// </summary>
    public class ConnectionTimeoutOptions : OptionsBase
    {
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
    }
}