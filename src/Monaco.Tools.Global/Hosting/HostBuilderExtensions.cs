namespace Monaco.Tools.Hosting
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class HostBuilderExtensions
    {
        public static async Task<int> RunCommandLineApplication(
            this IHostBuilder builder,
            string[] args,
            Action<CommandLineApplication> configure,
            CancellationToken cancellation = default)
        {
            var hostContext = new ApplicationContext(args);

            var app = new CommandLineApplication(PhysicalConsole.Singleton, Environment.CurrentDirectory);

            configure(app);

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton(app);

                services.AddSingleton(hostContext);

                services.AddSingleton<IApplicationHost, ApplicationHost>();
            });

            var host = builder.Build();

            return await host.Services.GetRequiredService<IApplicationHost>().Run(cancellation);
        }
    }
}
