namespace Monaco.Tools.Common
{
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.DependencyInjection;

    public interface IToolPlugin
    {
        string GetName();

        string GetCommandName();

        string GetPath();

        string GetDirectory();

        void ConfigureServices(IServiceCollection services);

        void ConfigureOptions(CommandLineApplication app);
    }
}
