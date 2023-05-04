﻿using SharedKernel.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Application.Cqrs.Middlewares
{
    /// <summary>
    /// Middleware that runs both on the command bus, as well as on the query and event bus
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface IMiddleware<TRequest> where TRequest : IBaseRequest
    {
        /// <summary>
        /// Middleware that runs both on the command bus, as well as on the query and event bus
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task> next);
    }

    /// <summary>
    /// Middleware that runs both on the command bus, as well as on the query and event bus
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Middleware that runs both on the command bus, as well as on the query and event bus
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task<TResponse>> next);
    }
}
