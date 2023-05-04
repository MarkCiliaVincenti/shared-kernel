﻿using SharedKernel.Application.Cqrs.Middlewares;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Cqrs.Middlewares;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class TimerMiddleware<TRequest, TResponse> : IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ITimeHandler _timeHandler;
    private readonly Stopwatch _timer;

    /// <summary> Constructor. </summary>
    public TimerMiddleware(ITimeHandler timeHandler)
    {
        _timeHandler = timeHandler;
        _timer = new Stopwatch();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task<TResponse>> next)
    {
        _timer.Start();

        var response = await next(request, cancellationToken);

        _timer.Stop();

        _timeHandler.Handle(request, _timer);

        return response;
    }
}