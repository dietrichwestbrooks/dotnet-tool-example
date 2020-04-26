namespace Monaco.Tools.Common
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class ToolPlugin : IToolPlugin
    {
        public abstract string GetName();

        public abstract string GetCommandName();

        public virtual string GetPath()
        {
            var codeBase = GetType().Assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            return Uri.UnescapeDataString(uri.Path);
        }

        public virtual string GetDirectory()
        {
            var directory = Path.GetDirectoryName(GetPath());
            Debug.Assert(directory != null);
            return directory;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void ConfigureOptions(CommandLineApplication app)
        {
        }
    }
}
