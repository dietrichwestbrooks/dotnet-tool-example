namespace Monaco.Tools
{
    using System.Threading.Tasks;
    using Hosting;
    using Install;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.AspNetCore.Hosting;
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

            var builder = CreateHostBuilder(args);

            return await builder.RunCommandLineApplication(args, app =>
            {
                app.Name = "Monaco Tool";
                app.Description = "Monaco development tools.";

                app.HelpOption();

                // app.VersionOption("0.0.0");

                app.Commands.Add(new CommandLineApplication<InstallCommand>());

                app.OnExecute(() =>
                {
                    Log.Error("Specify a command");
                    app.ShowHelp();
                });
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
