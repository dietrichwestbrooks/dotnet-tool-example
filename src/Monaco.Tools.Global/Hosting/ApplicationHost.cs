namespace Monaco.Tools.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using McMaster.Extensions.CommandLineUtils;
    using McMaster.Extensions.CommandLineUtils.Conventions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    internal class ApplicationHost : IApplicationHost
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHost _host;
        private readonly ApplicationContext _context;
        private readonly IEnumerable<IToolPlugin> _plugins;
        private readonly CommandLineApplication _app;

        public ApplicationHost(
            IServiceProvider serviceProvider, 
            CommandLineApplication app, 
            IHost host,
            ApplicationContext context, IEnumerable<IToolPlugin> plugins)
        {
            _serviceProvider = serviceProvider;
            _host = host;
            _context = context;
            _plugins = plugins;
            _app = app;
        }

        public async Task<int> Run(CancellationToken cancellationToken = default)
        {
            _app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(_serviceProvider);

            foreach (var convention in _serviceProvider.GetServices<IConvention>())
            {
                _app.Conventions.AddConvention(convention);
            }

            foreach (var plugin in _plugins)
            {
                plugin.ConfigureOptions(_app);
            }

            return await _app.ExecuteAsync(_context.Arguments, cancellationToken);
        }

        public async Task StartWebServer(CancellationToken cancellationToken = new CancellationToken())
        {
            await _host.RunAsync(cancellationToken);
        }
    }
}
