namespace Monaco.Tools.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ILogParser
    {
        Task<IEnumerable<LogModel>> Parse(string path);
    }
}
