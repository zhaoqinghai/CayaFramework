using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caya.Framework.Logging
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            var sw = Stopwatch.StartNew();
            try 
            {
                _logger.LogInformation("HTTP {HttpMethod}-- TraceId {TraceId}, RequestPath {Path}", httpContext.Request.Method, httpContext.TraceIdentifier, httpContext.Request.Path);
                if (httpContext.Request.ContentType.AsSpan().StartsWith("application/json"))
                {
                    var buffer = (await httpContext.Request.BodyReader.ReadAsync()).Buffer;
                    _logger.LogInformation("TraceId {TraceId}, RequestBody: {RequestJson}", httpContext.TraceIdentifier, Encoding.UTF8.GetString(buffer.FirstSpan));
                    httpContext.Request.BodyReader.AdvanceTo(buffer.Start, buffer.End);
                }
                await _next(httpContext);
                _logger.LogInformation("TraceId {TraceId}, Responded {StatusCode} in {ElapsedMilliseconds}ms", httpContext.TraceIdentifier, httpContext.Response.StatusCode, sw.ElapsedMilliseconds);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "TraceId {TraceId}", httpContext.TraceIdentifier);
            }
        }
    }

    public class RequstLogModel
    {
        public int StatusCode { get; set; }

        public string HttpMethod { get; set; }

        public string Path { get; set; }

        public long ElapsedMilliseconds { get; set; }
    }
}
