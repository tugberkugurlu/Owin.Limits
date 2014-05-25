namespace Owin.Limits
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Owin;

    public static partial class LimitsMiddleware
    {
        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="options">The max request content lenght options.</param>
        /// <returns>The middleware delegate.</returns>
        public static MidFunc MaxUrlLength(MaxUrlLengthOptions options)
        {
            options.MustNotNull("options");

            return
                next =>
                env =>
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
                };
        }
    }
}