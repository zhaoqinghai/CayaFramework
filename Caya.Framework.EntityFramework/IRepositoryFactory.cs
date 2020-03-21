using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.EntityFramework
{
    public interface IRepositoryFactory
    {
        CayaRepositroy<TDbContext> CreateReadRepo<TDbContext>() where TDbContext : CayaDbContext;

        CayaRepositroy<TDbContext> CreateWriteRepo<TDbContext>() where TDbContext : CayaDbContext;
    }
}
