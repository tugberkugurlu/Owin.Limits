namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Owin;

    internal class MaxUrlLengthMiddleware
    {
        private readonly Func<int> _getMaxUrlLength;
        private readonly Func<IDictionary<string, object>, Task> _next;

        public MaxUrlLengthMiddleware(Func<IDictionary<string, object>, Task> next, Func<int> getMaxUrlLength)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (getMaxUrlLength == null)
            {
                throw new ArgumentNullException("getMaxUrlLength");
            }
            _next = next;
            _getMaxUrlLength = getMaxUrlLength;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            var context = new OwinContext(environment);
            int maxUrlLength = _getMaxUrlLength();
            string unescapedUri = Uri.UnescapeDataString(context.Request.Uri.AbsoluteUri);

            if (unescapedUri.Length > maxUrlLength)
            {
                context.Response.StatusCode = 414;
                return;
            }
            await _next(environment);
        }
    }
}