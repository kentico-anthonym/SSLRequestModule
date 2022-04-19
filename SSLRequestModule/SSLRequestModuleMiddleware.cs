using CMS.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

public class SSLRequestModuleMiddleware
{
    private readonly RequestDelegate _next;

    public SSLRequestModuleMiddleware(RequestDelegate next)
    {
        _next = next;
        URLHelper.SSLUrlPort = 443;
    }

    public async Task Invoke(HttpContext context)
    {
        // Do something with context near the beginning of request processing.
        if (context.Request != null)
        {
            context.Request.Headers.TryGetValue("X-Forwarded-Proto", out var apacheHeader);
            context.Request.Headers.TryGetValue("X-Forwarded-Ssl", out var normalHeader);
            RequestContext.IsSSL = false;
            // Checks if the original request used HTTPS
            if (String.Equals(normalHeader, "on", StringComparison.OrdinalIgnoreCase) || String.Equals(apacheHeader, "https", StringComparison.OrdinalIgnoreCase))
            {
                RequestContext.IsSSL = true;
            }
        }
        await _next.Invoke(context);

        // Clean up.
    }
}

public static class SSLRequestModuleMiddlewareExtensions
{
    public static IApplicationBuilder UseSSLRequestModuleMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SSLRequestModuleMiddleware>();
    }
}


// https://docs.microsoft.com/en-us/aspnet/core/migration/http-modules?view=aspnetcore-6.0