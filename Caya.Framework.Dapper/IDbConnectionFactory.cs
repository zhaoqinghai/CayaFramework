using System;
using System.Data;

namespace Caya.Framework.Dapper
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateReadDbConnection(string key);

        IDbConnection CreateWriteDbConnection(string key);
    }
}
