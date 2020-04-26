namespace Monaco.Tools
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Hosting;
    using Install;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Welcome to Monaco Dev Tools!");

            var app = new CommandLineApplication(PhysicalConsole.Singleton, Environment.CurrentDirectory)
            {
                Name = "Monaco Tool", Description = "Monaco development tools."
            };

            app.HelpOption();

            // app.VersionOption("0.0.0");

            app.Commands.Add(new CommandLineApplication<InstallCommand>());

            app.OnExecute(() =>
            {
                Log.Error("Specify a command");
                app.ShowHelp();
            });

            var builder = CreateHostBuilder(args);

            return await builder.RunApplication<IApplicationRunner>(services =>
            {
                var context = new ApplicationContext(args);

                services.AddSingleton(app);

                services.AddSingleton(context);

                services.AddSingleton<IApplicationRunner, ApplicationRunner>();
            });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStaticWebAssets();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
