namespace Monaco.Tools.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class HostBuilderExtensions
    {
        public static async Task<int> RunApplication<TRunner>(
            this IHostBuilder builder,
            Action<IServiceCollection> configureServices,
            CancellationToken cancellation = default)
            where TRunner : IApplicationRunner
        {
            builder.ConfigureServices((context, services) => configureServices(services));

            var host = builder.Build();

            return await host.Services.GetRequiredService<TRunner>().Run(cancellation);
        }
    }
}
