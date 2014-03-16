Owin.Limits
===========

OWIN middleware to apply limits to an OWIN pipeline.

Install via [nuget].

Implemented:
 - Max bandwidth
 - Max concurrent requests
 - Connection timeout
 - Max query string
 
TODO:
 - MaxAllowedContentLength
 - MaxUrl (enforced by hosts?)
 - (Header limits)?

Example

Configuration values can be supplied as constants or with a delegate. The latter allows you to change the values at runtime, however you see fit.

```csharp
public class Startup
{
    public void Configuration(IAppBuilder builder)
    {
        //Constant settings
        builder
            .MaxBandwidth(10000) //bps
            .MaxConcurrentRequests(10)
            .ConnectionTimeout(TimeSpan.FromSeconds(10))
            .UseEtc(..);
            
        //dynamic settings
        builder
            .MaxBandwidth(() => 10000) //bps
            .MaxConcurrentRequests(() => 10)
            .ConnectionTimeout(() => TimeSpan.FromSeconds(10))
            .UseEtc(..);
    }
}
```

Pull requests gratefully accepted.

Questions or suggestions? Create an issue or [@randompunter] on twitter.

[nuget]: https://www.nuget.org/packages/Owin.Limits
[@randompunter]: http://twitter.com/randompunter
