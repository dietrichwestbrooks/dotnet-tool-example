namespace Monaco.Tools.Plugins
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Services;

    [Command(Name = "tool", Description = "Parses Monaco log files for EMDI events.")]
    public class ParseCommand
    {
        private readonly ILogParser _parser;

        public ParseCommand(ILogParser parser)
        {
            _parser = parser;
        }

        [Argument(0, "The path to the Monaco log files.")]
        [Required]
        public string Path { get; set; }

        private async Task OnExecuteAsync(CommandLineApplication app)
        {
            await _parser.Parse(Path);
        }
    }
}
