namespace Monaco.Tools.Common
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IToolApplication
    {
        Task StartWebServer(CancellationToken cancellationToken = default);
    }
}
