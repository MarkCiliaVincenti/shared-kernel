﻿using System.Diagnostics;

namespace SharedKernel.Infrastructure.Cqrs.Middlewares;

/// <summary>  </summary>
public interface ITimeHandler
{
    /// <summary>  </summary>
    public void Handle<TRequest>(TRequest request, Stopwatch timer);
}