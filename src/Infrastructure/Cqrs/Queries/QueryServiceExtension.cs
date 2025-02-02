﻿using System;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure.System;
using System.Reflection;
using SharedKernel.Application.Cqrs.Queries;
using SharedKernel.Application.Validator;
using SharedKernel.Infrastructure.Cqrs.Middlewares;
using SharedKernel.Infrastructure.Cqrs.Queries.InMemory;
using SharedKernel.Infrastructure.Validators;

namespace SharedKernel.Infrastructure.Cqrs.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public static class QueryServiceExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="infrastructureAssembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddQueriesHandlers(this IServiceCollection services, params Assembly[] infrastructureAssembly)
        {
            foreach (var assembly in infrastructureAssembly)
                services.AddFromAssembly(assembly, typeof(IQueryRequestHandler<,>));

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="queryHandlerTypes"></param>
        /// <returns></returns>
        public static IServiceCollection AddQueriesHandlers(this IServiceCollection services, params Type[] queryHandlerTypes)
        {
            foreach (var queryHandlerType in queryHandlerTypes)
                services.AddFromAssembly(queryHandlerType.Assembly, typeof(IQueryRequestHandler<,>));

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryQueryBus(this IServiceCollection services)
        {
            return services
                .AddQueryBus()
                .AddTransient<IQueryBus, InMemoryQueryBus>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddQueryBus(this IServiceCollection services)
        {
            return services
                .AddTransient<ExecuteMiddlewaresService>()
                .AddTransient(typeof(IEntityValidator<>), typeof(FluentValidator<>));
        }
    }
}
