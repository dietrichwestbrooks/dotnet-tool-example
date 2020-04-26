namespace Monaco.Tools.Hosting
{
    using McMaster.Extensions.CommandLineUtils.Abstractions;

    internal class ApplicationContext : CommandLineContext
    {
        public ApplicationContext(string[] args)
        {
            Arguments = args;
        }
    }
}
