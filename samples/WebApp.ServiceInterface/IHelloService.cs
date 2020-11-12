using System;
using System.Threading.Tasks;

namespace WebApp.ServiceInterface
{
    public interface IHelloService
    {
        string SayHello();

        Task Insert();
    }
}
