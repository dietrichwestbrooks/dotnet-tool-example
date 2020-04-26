namespace Monaco.Tools.Hosting
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    [Command(Name = "tool", Description = "Runs a tool given a path to the assembly (for Debug only).")]
    public class ToolCommand
    {
        [Argument(0, "The path to the assembly file for the tool.")]
        [Required]
        public string Path { get; set; }

        private async Task OnExecuteAsync(CommandLineApplication app)
        {
            await Task.CompletedTask;
        }
    }
}
