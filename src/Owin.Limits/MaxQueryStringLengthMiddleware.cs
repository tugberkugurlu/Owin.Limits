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

            var context = new OwinContext(environment);
            QueryString queryString = context.Request.QueryString;
            if (queryString.HasValue) 
            {
                int maxQueryStringLength = _options.GetMaxQueryStringLength();
                string unescapedQueryString = Uri.UnescapeDataString(queryString.Value);
                if (unescapedQueryString.Length > maxQueryStringLength)
                {
                    context.Response.StatusCode = 414;
                    context.Response.ReasonPhrase = _options.LimitReachedReasonPhrase(414);
                    return;
                }
            }

            await _next(environment);
        }
    }
}