namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin.Limits.Annotations;

    [UsedImplicitly]
    internal class ConnectionTimeoutMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly ConnectionTimeoutOptions _options;

        public ConnectionTimeoutMiddleware(Func<IDictionary<string, object>, Task> next
            , ConnectionTimeoutOptions options)
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
            _options.Tracer.AsVerbose("{0} starts processing request.".FormattedWith(GetType().Name));

            var context = new OwinContext(environment);
            Stream requestBodyStream = context.Request.Body ?? Stream.Null;
            Stream responseBodyStream = context.Response.Body;

            _options.Tracer.AsVerbose("Configure timeouts.");
            TimeSpan connectionTimeout = _options.GetTimeout();
            context.Request.Body = new TimeoutStream(requestBodyStream, connectionTimeout, _options.Tracer);
            context.Response.Body = new TimeoutStream(responseBodyStream, connectionTimeout, _options.Tracer);

            _options.Tracer.AsVerbose("Request with configured timeout forwarded.");
            await _next(environment);

            context.Request.Body = requestBodyStream;
            context.Response.Body = responseBodyStream;
            _options.Tracer.AsVerbose("{0} finished processing.".FormattedWith(GetType().Name));
        }
    }
}