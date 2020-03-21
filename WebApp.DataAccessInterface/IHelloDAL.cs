using System;
using System.Threading.Tasks;

namespace WebApp.DataAccessInterface
{
    public interface IHelloDAL
    {
        string SayHello();

        Task Insert();
    }
}
