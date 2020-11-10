using System;
using System.Collections.Generic;
using System.Text;
using Nest;

namespace Caya.Framework.ElasticSearch
{
    public interface IElasticSearchClientProvider
    {
        ElasticClient GetElasticClient(string name);

        ElasticClient Default { get; }
    }
}
