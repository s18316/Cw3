using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            //our code
            //
            var tmp = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            StringBuilder sb = new StringBuilder();

            var bodyStream = string.Empty;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStream = await reader.ReadToEndAsync();
            }

            sb.Append(httpContext.Request.Method);
            sb.Append("\n");
            sb.Append(httpContext.Request.Path);
            sb.Append("\n");
            sb.Append(bodyStream);
            sb.Append("\n");
            sb.Append(httpContext.Request.QueryString);

             File.AppendAllText(tmp +"\\logMid.txt" , sb.ToString());
               
            await _next(httpContext);
        }
    }
}