using Dapper;
using Caya.Framework.EntityFramework;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DataAccessInterface;
using WebApp.Entity;

namespace WebApp.DataAccess
{
    public class HelloDAL : IHelloDAL
    {
        private readonly ILogger<HelloDAL> _logger;
        private readonly RepositoryFactoryResolver _resolver;
        public HelloDAL(ILogger<HelloDAL> logger, RepositoryFactoryResolver resolver) 
        {
            _logger = logger;
            _resolver = resolver;
        }
        public async Task Insert()
        {
            using var repo = _resolver(DbKind.Postgresql).CreateRepo<HelloDbContext>();
            await repo.InsertAsync<User>(new User()
            {
                Name = "赵庆海",
                Age = 22,
                Id = Guid.NewGuid()
            });
            await repo.SaveChangesAsync();
        }
        public string SayHello()
        {
            using var repo = _resolver(DbKind.Postgresql).CreateRepo<HelloDbContext>();
            var a = repo.GetQuery<User>().ToList();
            //using var repo = _factory.CreateReadRepo<HelloDbContext>();
            ////repo.InsertRange(list);
            ////var name = repo.GetQuery<User>().Select(item => item.Name).FirstOrDefault();
            //var a = repo.QuerySql<User>("select * from [User] where Age in @AgeList", new { AgeList = new[] { 18, 20 } }).ToList();
            return "name";
            //using (var connection = _connectionFactory.CreateReadDbConnection("Test0"))
            //{
            //    return connection.QueryFirstOrDefault<string>("select top(1) Name from [User]");
            //}
        }
    }
}
