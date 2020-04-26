namespace Monaco.Tools.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;
    using Common;

    internal interface IApplicationHost : IToolApplication
    {
        Task<int> Run(CancellationToken cancellationToken = default);
    }
}
