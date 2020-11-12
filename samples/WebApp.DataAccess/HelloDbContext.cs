using Caya.Framework.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.Entity;

namespace WebApp.DataAccess
{
    public class HelloDbContext : CayaDbContext
    {
        public HelloDbContext(DbContextOptions options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
