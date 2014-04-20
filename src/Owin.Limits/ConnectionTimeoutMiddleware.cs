namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin.Limits.Annotations;

    [UsedImplicitly]
    internal class ConnectionTimeoutMiddleware : MiddlewareBase
    {
        private readonly ConnectionTimeoutOptions _options;

        public ConnectionTimeoutMiddleware(Func<IDictionary<string, object>, Task> next, ConnectionTimeoutOptions options)
            : base(next.ToAppFunc(), options.Tracer)
        {
            _options = options;
        }

        protected override Task InvokeInternal(AppFunc next, IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);
            Stream requestBodyStream = context.Request.Body ?? Stream.Null;
            Stream responseBodyStream = context.Response.Body;

            _options.Tracer.AsVerbose("Configure timeouts.");
            TimeSpan connectionTimeout = _options.GetTimeout();
            context.Request.Body = new TimeoutStream(requestBodyStream, connectionTimeout, _options.Tracer);
            context.Response.Body = new TimeoutStream(responseBodyStream, connectionTimeout, _options.Tracer);

            _options.Tracer.AsVerbose("Request with configured timeout forwarded.");
            return next(environment);
        }
    }
}