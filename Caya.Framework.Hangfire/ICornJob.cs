using System;
using System.Threading.Tasks;

namespace Caya.Framework.Hangfire
{
    public interface IBackgroundJob
    {
        Task ExecuteAsync();

        TimeSpan Delay { get; }
    }

    public interface ICornJob
    {
        Task ExecuteAsync();

        string Corn { get; }
    }
}
