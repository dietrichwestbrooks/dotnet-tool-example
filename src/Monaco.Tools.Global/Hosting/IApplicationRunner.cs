namespace Monaco.Tools.Hosting
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IApplicationRunner
    {
        Task<int> Run(CancellationToken cancellationToken = default);
    }
}
