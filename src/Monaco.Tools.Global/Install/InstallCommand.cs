namespace Monaco.Tools.Install
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    [Command(Name = "install", Description = "Installs a Monaco tool from a package repository.")]
    internal class InstallCommand
    {
        private readonly IToolInstaller _installer;

        public InstallCommand(IToolInstaller installer)
        {
            _installer = installer;
        }

        [Argument(0, "The name of the tool to install.")]
        [Required]
        public string Name { get; set; }

        [Argument(1, "The version of tool to install.")]
        [Required]
        public string Version { get; set; }

        [Option("-s|--source <source>", CommandOptionType.SingleValue, Description = "The package repository location.")]
        [Required]
        public string Source { get; set; }

        private async Task OnExecuteAsync(CommandLineApplication app)
        {
            await _installer.Install(Name, Version, Source);
        }
    }
}
