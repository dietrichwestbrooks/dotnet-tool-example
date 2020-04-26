namespace Monaco.Tools.Hosting
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using McMaster.Extensions.CommandLineUtils;
    using McMaster.Extensions.CommandLineUtils.Conventions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    internal class ApplicationRunner : IApplicationRunner
    {
        private readonly ApplicationContext _context;
        private readonly IHost _host;
        private readonly IEnumerable<IToolPlugin> _plugins;
        private readonly CommandLineApplication _app;

        public ApplicationRunner(
            IHost host,
            CommandLineApplication app,
            ApplicationContext context,
            IEnumerable<IToolPlugin> plugins)
        {
            _host = host;
            _context = context;
            _plugins = plugins;
            _app = app;
        }

        public async Task<int> Run(CancellationToken cancellationToken = default)
        {
            _app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(_host.Services);

            foreach (var convention in _host.Services.GetServices<IConvention>())
            {
                _app.Conventions.AddConvention(convention);
            }

            foreach (var plugin in _plugins)
            {
                plugin.ConfigureOptions(_app);
            }

            return await _app.ExecuteAsync(_context.Arguments, cancellationToken);
        }
    }
}