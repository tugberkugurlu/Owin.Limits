namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin.Limits.Annotations;

    [UsedImplicitly]
    internal class MaxUrlLengthMiddleware : MiddlewareBase
    {
        private readonly MaxUrlLengthOptions _options;

        public MaxUrlLengthMiddleware(Func<IDictionary<string, object>, Task> next, MaxUrlLengthOptions options)
            : base(next.ToAppFunc(), options.Tracer)
        {
            _options = options;
        }

        protected override async Task InvokeInternal(AppFunc next, IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);
            int maxUrlLength = _options.GetMaxUrlLength();
            string unescapedUri = Uri.UnescapeDataString(context.Request.Uri.AbsoluteUri);

            _options.Tracer.AsVerbose("Checking request url length.");
            if (unescapedUri.Length > maxUrlLength)
            {
                _options.Tracer.AsInfo("Url \"{0}\"(Length: {2}) exceeds allowed length of {1}. Request rejected.".FormatWith(unescapedUri, maxUrlLength,
                    unescapedUri.Length));
                context.Response.StatusCode = 414;
                context.Response.ReasonPhrase = _options.LimitReachedReasonPhrase(context.Response.StatusCode);
                return;
            }
            _options.Tracer.AsVerbose("Check passed. Request forwarded.");
            await next(environment);
            _options.Tracer.AsVerbose("Processing finished.");
        }
    }
}