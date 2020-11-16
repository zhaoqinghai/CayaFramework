using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.EntityFramework
{
    public interface IRepositoryFactory
    {
        CayaRepository<TDbContext> CreateReadRepo<TDbContext>() where TDbContext : CayaDbContext;

        CayaRepository<TDbContext> CreateWriteRepo<TDbContext>() where TDbContext : CayaDbContext;
    }
}
