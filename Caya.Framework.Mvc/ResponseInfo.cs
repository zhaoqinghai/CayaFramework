using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Mvc
{
    public class ResponseInfo
    {
        public int Code { get; set; }

        public string Message { get; set; }
    }

    public class ResponseInfo<T> : ResponseInfo
    {
        public T Data { get; set; }
    }

    public class ResponsePageInfo<T> : ResponseInfo<List<T>>
    {
        public long TotalCount { get; set; }
    }
}
