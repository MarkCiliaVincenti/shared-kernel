﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.Cqrs.Middlewares;
using SharedKernel.Application.Events;
using SharedKernel.Application.RetryPolicies;
using SharedKernel.Domain.Tests.Users;
using SharedKernel.Infrastructure.Cqrs.Middlewares;
using SharedKernel.Infrastructure.Events;
using SharedKernel.Infrastructure.RetryPolicies;
using SharedKernel.Integration.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SharedKernel.Integration.Tests.Events.SynchronousEventBus
{
    public class SynchronousEventBusTests : InfrastructureTestCase
    {
        protected override string GetJsonFile()
        {
            return "Events/SynchronousEventBus/appsettings.synchronous.json";
        }

        /// <summary>RetryPolicyMiddleware Exception handler</summary>
        public class RetryPolicyExceptionHandler : IRetryPolicyExceptionHandler
        {
            /// <summary>Check if an exception has to be retried</summary>
            /// <param name="exception"></param>
            /// <returns></returns>
            public bool NeedToRetryTheException(Exception exception) => true;
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services
                .AddSynchronousEventBus()
                .AddDomainEvents(typeof(UserCreated))
                .AddDomainEventsSubscribers(typeof(SetCountWhenUserCreatedSubscriber))
                .AddDomainEventSubscribers()
                .AddSingleton<PublishUserCreatedDomainEvent>()

                .AddTransient(typeof(IMiddleware<>), typeof(ValidationMiddleware<>))
                .AddTransient(typeof(IMiddleware<,>), typeof(ValidationMiddleware<,>))

                .AddTransient<IRetriever, PollyRetriever>()
                .AddTransient<IRetryPolicyExceptionHandler, RetryPolicyExceptionHandler>()
                .AddPollyRetry(Configuration)

                .AddTransient(typeof(IMiddleware<>), typeof(RetryPolicyMiddleware<>))
                .AddTransient(typeof(IMiddleware<,>), typeof(RetryPolicyMiddleware<,>))


                .AddHttpContextAccessor();
        }

        [Fact]
        public async Task PublishDomainEventFromSynchronousEventBus()
        {
            var httpContextAccessor = GetRequiredService<IHttpContextAccessor>();

            if (httpContextAccessor != default)
            {
#if !NET461 && !NETSTANDARD
                httpContextAccessor.HttpContext ??= new DefaultHttpContext();
#endif
                httpContextAccessor.HttpContext.User =
                    new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("Name", "Peter") }));

                httpContextAccessor.HttpContext.Request.Headers.Add("Authorization", "Prueba");
            }

            var user1 = UserMother.Create();
            var user2 = UserMother.Create();
            var user3 = UserMother.Create();
            var user4 = UserMother.Create();

            var domainEvents = user1.PullDomainEvents();
            domainEvents.AddRange(user2.PullDomainEvents());
            domainEvents.AddRange(user3.PullDomainEvents());
            domainEvents.AddRange(user4.PullDomainEvents());

            var eventBus = GetRequiredService<IEventBus>();
            var tasks = new List<Task>();
            const int total = 5;
            for (var i = 0; i < total; i++)
            {
                tasks.Add(eventBus.Publish(domainEvents, CancellationToken.None));
            }

            await Task.WhenAll(tasks);

            // Esperar a que terminen los manejadores de los eventos junto con la política de reintentos
            var singletonValueContainer = GetRequiredService<PublishUserCreatedDomainEvent>();
            var salir = 0;
            while (singletonValueContainer.Total != total * 4 && salir <= 50)
            {
                await Task.Delay(50, CancellationToken.None);
                salir++;
            }

            singletonValueContainer.Users.Should().Contain(u => u == user1.Id);
            singletonValueContainer.Users.Should().Contain(u => u == user2.Id);
            singletonValueContainer.Users.Should().Contain(u => u == user3.Id);
            singletonValueContainer.Users.Should().Contain(u => u == user4.Id);
        }
    }
}
