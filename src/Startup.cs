using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TAlex.ImageProxy;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Startup))]

namespace TAlex.ImageProxy
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddOptions<ProxySettings>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("ProxySettings").Bind(settings));

            FixConfiguration(builder.Services);

            builder.Services.AddSingleton<IImageProxyService, ImageProxyService>();
        }

        public static void FixConfiguration(IServiceCollection services)
        {
            var providers = new List<IConfigurationProvider>();

            foreach (var descriptor in services.Where(descriptor => descriptor.ServiceType == typeof(IConfiguration)).ToList())
            {
                var existingConfiguration = descriptor.ImplementationInstance as IConfigurationRoot;
                if (existingConfiguration is null)
                {
                    continue;
                }
                providers.AddRange(existingConfiguration.Providers);
                services.Remove(descriptor);
            }

            var executioncontextoptions = services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;
            var currentDirectory = executioncontextoptions.AppDirectory;

            var config = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            providers.AddRange(config.Build().Providers);

            services.AddSingleton<IConfiguration>(new ConfigurationRoot(providers));
        }
    }
}
