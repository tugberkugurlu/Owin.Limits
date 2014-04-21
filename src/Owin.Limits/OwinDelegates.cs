namespace Owin.Limits
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// OWIN application func.
    /// </summary>
    /// <param name="environment">The environment dictionary.</param>
    public delegate Task AppFunc(IDictionary<string, object> environment);

    /// <summary>
    /// OWIN middleware func.
    /// </summary>
    /// <param name="appFunc">An OWIN application func.</param>
    public delegate AppFunc MidFunc(AppFunc appFunc);
}