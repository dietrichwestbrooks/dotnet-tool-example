namespace Monaco.Tools.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Hosting;

    internal class WebServer : IWebServer
    {
        private readonly IHost _host;

        public WebServer(IHost host)
        {
            _host = host;
        }

        public async Task Start(CancellationToken cancellationToken = new CancellationToken())
        {
            await _host.RunAsync(cancellationToken);
        }
    }
}
