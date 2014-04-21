namespace Owin.Limits
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Owin;

    /// <summary>
    /// OWIN middleware to limit the length of a url.
    /// </summary>
    public static class MaxUrlLengthMiddlware
    {
        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="maxUrlLength">Maximum length of the URL.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxUrlLength(this Action<MidFunc> builder, int maxUrlLength)
        {
            return MaxUrlLength(builder, () => maxUrlLength);
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="getMaxUrlLength">A delegate to get the maximum URL length.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxUrlLength(this Action<MidFunc> builder, Func<int> getMaxUrlLength)
        {
            return MaxUrlLength(builder, new MaxUrlLengthOptions(getMaxUrlLength));
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="options">The max url length options.</param>
        /// <returns>The OWIN builder instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder or options</exception>
        public static Action<MidFunc> MaxUrlLength(this Action<MidFunc> builder, MaxUrlLengthOptions options)
        {
            builder.MustNotNull("builder");
            options.MustNotNull("options");

            builder(next => env =>
            {
                var context = new OwinContext(env);
                int maxUrlLength = options.GetMaxUrlLength();
                string unescapedUri = Uri.UnescapeDataString(context.Request.Uri.AbsoluteUri);

                options.Tracer.AsVerbose("Checking request url length.");
                if (unescapedUri.Length > maxUrlLength)
                {
                    options.Tracer.AsInfo(
                        "Url \"{0}\"(Length: {2}) exceeds allowed length of {1}. Request rejected.",
                        unescapedUri,
                        maxUrlLength,
                        unescapedUri.Length);
                    context.Response.StatusCode = 414;
                    context.Response.ReasonPhrase = options.LimitReachedReasonPhrase(context.Response.StatusCode);
                    return Task.FromResult(0);
                }
                options.Tracer.AsVerbose("Check passed. Request forwarded.");
                return next(env);
            });
            return builder;
        }
    }
}