Owin.Limits
===========

OWIN middleware to apply limits to an OWIN pipeline. Install via [nuget].

#### Implemented:
 - Max bandwidth
 - Max concurrent requests
 - Connection timeout
 - Max query string
 - Max request content length
 - Max url length
 
#### TODO:
 - Header limits?
 - Per request limits

#### IAppBuilder

Owin.dll and IAppBuilder is deprecated. As of version 2.0.0, Owin.Limits no longer depends on owin.dll. To provide compatibility with IAppBuilder, add the following class to your application:

```csharp
namespace Owin
{
    using System;
    using Owin.Limits;

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
```

#### Examples

Configuration values can be supplied as constants or with a delegate. The latter allows you to change the values at runtime. Use which ever you see fit. This code assumes you have the above `AppBuilderExtensions` class in your application. 


```csharp
public class Startup
{
    public void Configuration(IAppBuilder builder)
    {
        //static settings
        builder.Use()
            .MaxBandwidth(10000) //bps
            .MaxConcurrentRequests(10)
            .ConnectionTimeout(TimeSpan.FromSeconds(10))
            .MaxQueryStringLength(15) //Unescaped QueryString
            .MaxRequestContentLength(15)
            .MaxUrlLength(20)
            .UseEtc(..);
            
        //dynamic settings
        builder.Use()
            .MaxBandwidth(() => 10000) //bps
            .MaxConcurrentRequests(() => 10)
            .ConnectionTimeout(() => TimeSpan.FromSeconds(10))
            .MaxQueryStringLength(() => 15)
            .MaxRequestContentLength(() => 15)
            .MaxUrlLength(() => 20)
            .UseEtc(..);
    }
}
```


Questions or suggestions? Create an issue or [@randompunter] on twitter.

Pull requests gratefully accepted.

Thanks to the following contributors!
 - [Stefan Ossendorf](https://github.com/StefanOssendorf) ([@Pherenetic](https://twitter.com/Pherenetic))

[nuget]: https://www.nuget.org/packages/Owin.Limits
[@randompunter]: http://twitter.com/randompunter
