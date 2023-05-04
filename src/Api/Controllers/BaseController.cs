﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using SharedKernel.Application.Cqrs.Commands;
using SharedKernel.Application.Cqrs.Queries;

namespace SharedKernel.Api.Controllers
{
    /// <summary>
    /// Base controller
    /// </summary>
    [ApiController, Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets the command bus
        /// </summary>
        protected ISender CommandBus => HttpContext.RequestServices.GetRequiredService<ISender>();

        /// <summary>
        /// Gets de query bus
        /// </summary>
        protected IQueryBus QueryBus => HttpContext.RequestServices.GetRequiredService<IQueryBus>();


        /// <summary>
        /// Creates a <see cref="ActionResult&lt;Value&gt;"/> object that produces an <see cref="StatusCodes.Status200OK"/> response.
        /// </summary>
        /// <returns>The created <see cref="ActionResult&lt;Value&gt;"/> for the response.</returns>
        protected ActionResult<T> OkTyped<T>(T result) => Ok(result);

        /// <summary>
        /// Creates a <see cref="IActionResult"/> object that produces an empty <see cref="StatusCodes.Status200OK"/> response.
        /// </summary>
        /// <returns>The created <see cref="IActionResult"/> for the response.</returns>
        protected IActionResult OkTyped() => Ok();

        /// <summary>
        /// Read a file an return in streaming
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="cancellationToken"></param>
        protected async Task Streaming(string filePath, CancellationToken cancellationToken)
        {
            Response.StatusCode = 200;
            Response.Headers.Add(HeaderNames.ContentDisposition, $"attachment; filename=\"{Path.GetFileName(filePath)}\"");
            Response.Headers.Add(HeaderNames.ContentType, "application/octet-stream");
            await using var inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var outputStream = Response.Body;
            const int bufferSize = 1 << 10;
            var buffer = new byte[bufferSize];
            while (true)
            {
                var bytesRead = await inputStream.ReadAsync(buffer, 0, bufferSize, cancellationToken);
                if (bytesRead == 0)
                    break;

                await outputStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
            }
            await outputStream.FlushAsync(cancellationToken);
        }
    }
}
