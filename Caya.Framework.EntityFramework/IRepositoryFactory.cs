using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.EntityFramework
{
    public interface IRepositoryFactory
    {
        CayaRepository<TDbContext> CreateRepo<TDbContext>() where TDbContext : CayaDbContext;
    }
}
