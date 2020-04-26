namespace Monaco.Tools.Common
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IWebServer
    {
        Task Start(CancellationToken cancellationToken = default);
    }
}
