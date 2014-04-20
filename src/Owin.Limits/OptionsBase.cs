namespace Owin.Limits
{
    /// <summary>
    /// Base options class
    /// </summary>
    public abstract class OptionsBase
    {
        private Tracer _tracer;

        /// <summary>
        /// Gets or sets the delegate to trace the middleware.
        /// </summary>
        public Tracer Tracer
        {
            get { return _tracer ?? DefaultDelegateHelper.DefaultTracer; }
            set { _tracer = value; }
        }
    }
}