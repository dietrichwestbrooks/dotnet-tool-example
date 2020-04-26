namespace Monaco.Tools.Plugins
{
    using Common;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    public class EmdiToolPlugin : ToolPlugin
    {
        public override string GetName() => "Monaco.Tool.Emdi";

        public override string GetCommandName() => "emdi";

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogParser, LogParser>();
        }

        public override void ConfigureOptions(CommandLineApplication app)
        {
            app.Commands.Add(new CommandLineApplication<ParseCommand>());
        }
    }
}
