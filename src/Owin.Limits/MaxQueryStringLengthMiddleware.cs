namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin.Limits.Annotations;

    [UsedImplicitly]
    internal class MaxQueryStringLengthMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly MaxQueryStringLengthOptions _options;

        public MaxQueryStringLengthMiddleware(Func<IDictionary<string, object>, Task> next,
            MaxQueryStringLengthOptions options)
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

        [UsedImplicitly]
        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            _options.Tracer.AsVerbose("{0} starts processing request".FormattedWith(GetType().Name));

            var context = new OwinContext(environment);
            QueryString queryString = context.Request.QueryString;
            if (queryString.HasValue)
            {
                int maxQueryStringLength = _options.GetMaxQueryStringLength();
                string unescapedQueryString = Uri.UnescapeDataString(queryString.Value);
                _options.Tracer.AsVerbose("Querystring of request with an unescaped length of {0}".FormattedWith(unescapedQueryString.Length));
                if (unescapedQueryString.Length > maxQueryStringLength)
                {
                    _options.Tracer.AsInfo("Querystring (Length {0}) too long (allowed {1}). Request rejected.".FormattedWith(unescapedQueryString.Length,
                        maxQueryStringLength));
                    context.Response.StatusCode = 414;
                    context.Response.ReasonPhrase = _options.LimitReachedReasonPhrase(context.Response.StatusCode);
                    return;
                }
                _options.Tracer.AsVerbose("Querystring length check passed.");
            }
            else
            {
                _options.Tracer.AsVerbose("No querystring.");
            }

            _options.Tracer.AsVerbose("{0} finished processing request. Request is forwarded.".FormattedWith(GetType().Name));
            await _next(environment);
        }
    }
}