using System;
using System.Threading.Tasks;
using WebApp.DataAccessInterface;
using WebApp.ServiceInterface;

namespace WebApp.Service
{
    public class HelloService : IHelloService
    {
        private readonly IHelloDAL _dal;
        public HelloService(IHelloDAL dal)
        {
            _dal = dal;
        }

        public async Task Insert()
        {
            await _dal.Insert();
        }

        public string SayHello()
        {

            return _dal.SayHello();
        }
    }
}
