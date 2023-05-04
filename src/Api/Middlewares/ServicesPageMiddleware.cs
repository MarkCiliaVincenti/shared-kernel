﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;

namespace SharedKernel.Api.Middlewares
{
    /// <summary> Service page extensions. </summary>
    public static class ServicesPageMiddleware
    {
        /// <summary> Adds a route /services to show the dependency container. </summary>
        public static IApplicationBuilder UseSharedKernelServicesPage(this IApplicationBuilder app, IServiceCollection services)
        {
            app.Map("/services", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append($"<h1>Registered {services.Count} Services</h1>");
                sb.Append($"<h1>Registered {services.Count(s => s.Lifetime == ServiceLifetime.Singleton)} Singleton Services</h1>");
                sb.Append($"<h1>Registered {services.Count(s => s.Lifetime == ServiceLifetime.Scoped)} Scoped Services</h1>");
                sb.Append($"<h1>Registered {services.Count(s => s.Lifetime == ServiceLifetime.Transient)} Transient Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Lifetime</th><th>Class</th><th>Interface</th></tr>");

                sb.Append("</thead><tbody>");
                var total = services
                    .OrderBy(s => s.Lifetime)
                    .ThenBy(s => s.ImplementationType?.FullName)
                    .ThenBy(s => s.ServiceType.FullName);

                foreach (var svc in total)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));

            return app;
        }
    }
}
