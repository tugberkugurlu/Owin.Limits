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

#### Examples

Configuration values can be supplied as constants or with a delegate. The latter allows you to change the values at runtime, however you see fit.

```csharp
public class Startup
{
    public void Configuration(IAppBuilder builder)
    {
        //static settings
        builder
            .MaxBandwidth(10000) //bps
            .MaxConcurrentRequests(10)
            .ConnectionTimeout(TimeSpan.FromSeconds(10))
            .MaxQueryStringLength(15) //Unescaped QueryString
            .MaxRequestContentLength(15)
            .MaxUrlLength(20)
            .UseEtc(..);
            
        //dynamic settings
        builder
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

Thanks to the follow contributors!
 - [Stefan Ossendorf](https://github.com/StefanOssendorf) ([@Pherenetic](https://twitter.com/Pherenetic))

[nuget]: https://www.nuget.org/packages/Owin.Limits
[@randompunter]: http://twitter.com/randompunter
