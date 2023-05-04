﻿using SharedKernel.Application.Cqrs.Middlewares;
using SharedKernel.Domain.Events;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Cqrs.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public class TimerMiddleware<TRequest> : IMiddleware<TRequest> where TRequest : IBaseRequest
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
        public async Task Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task> next)
        {
            _timer.Start();

            await next(request, cancellationToken);

            _timer.Stop();

            _timeHandler.Handle(request, _timer);
        }
    }
}
