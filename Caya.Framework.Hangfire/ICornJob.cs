using System;
using System.Threading.Tasks;

namespace Caya.Framework.Hangfire
{
    public interface ICornJob
    {
        Task ExecuteAsync();

        string Corn { get; }
    }
}
