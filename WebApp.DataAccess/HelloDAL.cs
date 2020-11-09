using Dapper;
using Caya.Framework.Dapper;
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
        private readonly IRepositoryFactory _factory;
        private readonly IDbConnectionFactory _connectionFactory;
        public HelloDAL(IRepositoryFactory factory, ILogger<HelloDAL> logger, IDbConnectionFactory connectionFactory) 
        {
            _logger = logger;
            _factory = factory;
            _connectionFactory = connectionFactory;
        }
        public async Task Insert()
        {
            using var repo = _factory.CreateReadRepo<HelloDbContext>();
            await repo.InsertAsync<User>(new User()
            {
                Name = "赵庆海",
                Age = 22,
                Id = Guid.NewGuid()
            });
        }
        public string SayHello()
        {
            using var connection = _connectionFactory.CreateReadDbConnection("Test0");
            using var repo = _factory.CreateReadRepo<HelloDbContext>();
            var list = new List<User>();
            for (int i = 0; i < 50015; i++)
            {
                list.Add(new User()
                {
                    Name = $"zqh{i}",
                    Age = 18,
                    Id = Guid.NewGuid()
                });
            }

            connection.BatchInsertAsync(list).GetAwaiter().GetResult();
            var name = repo.GetQuery<User>().Select(item => item.Name).FirstOrDefault();
            var a = repo.QuerySql<User>("select * from [User] where Age in @AgeList", new { AgeList = new[] { 18, 20 } }).ToList();
            return name;
            //using (var connection = _connectionFactory.CreateReadDbConnection("Test0"))
            //{
            //    return connection.QueryFirstOrDefault<string>("select top(1) Name from [User]");
            //}
        }
    }
}
