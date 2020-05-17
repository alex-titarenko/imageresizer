using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(TAlex.ImageProxy.Startup))]

namespace TAlex.ImageProxy
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder
                .Services
                .AddOptions<ProxySettings>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind("ProxySettings", settings); });

            builder.Services.AddSingleton<IImageProxyService, ImageProxyService>();
        }
    }
}
