namespace Owin.Limits
{
    using System;

    /// <summary>
    /// Options for limiting the max bandwidth.
    /// </summary>
    public class MaxBandwidthOptions : OptionsBase
    {
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
    }
}