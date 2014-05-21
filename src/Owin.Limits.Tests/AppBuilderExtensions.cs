namespace Owin.Limits
{
    using System;

    internal static class AppBuilderExtensions
    {
        internal static Action<MidFunc> Use(this IAppBuilder builder)
        {
            return middleware => builder.Use(middleware);
        }

        internal static IAppBuilder Use(this Action<MidFunc> middleware, IAppBuilder builder)
        {
            return builder;
        }
    }
}