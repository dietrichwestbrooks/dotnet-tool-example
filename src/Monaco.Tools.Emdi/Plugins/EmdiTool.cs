namespace Monaco.Tools.Plugins
{
    using System.Diagnostics;
    using System.Web;
    using Common;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    public class EmdiTool : ToolPlugin
    {
        private readonly IWebServer _webServer;

        public EmdiTool(IWebServer webServer)
        {
            _webServer = webServer;
        }

        public override string GetName() => "Monaco.Tool.Emdi";

        public override string GetCommandName() => "emdi";

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogParser, LogParser>();
        }

        public override void ConfigureOptions(CommandLineApplication app)
        {
            app.Command("emdi", configurator =>
            {
                configurator.Description = "EMDI development and troubleshooting tools.";

                configurator.Command("parse", configurator =>
                {
                    configurator.Description = "Parses then displays EMDI events from Monaco log.";

                    var path = configurator.Argument("path", "The path to the Monaco log files.");

                    configurator.OnExecuteAsync(async cancellationToken =>
                    {
                        var task = _webServer.Start(cancellationToken);

                        Process.Start(
                            new ProcessStartInfo(
                                    $"http://localhost:9000/emdi/parse?path={HttpUtility.UrlEncode(path.Value)}")
                                {UseShellExecute = true});

                        await task;
                    });
                });
            });
        }
    }
}
