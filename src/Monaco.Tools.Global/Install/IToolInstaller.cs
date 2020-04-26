namespace Monaco.Tools.Install
{
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IToolInstaller
    {
        Task Install(string name, string version, string source, CancellationToken cancellationToken = default);
    }
}
