﻿using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.Cqrs.Commands;
using SharedKernel.Application.Cqrs.Middlewares;
using SharedKernel.Application.Validator;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Cqrs.Commands;
using SharedKernel.Infrastructure.Cqrs.Middlewares;
using SharedKernel.Infrastructure.Validators;
using SharedKernel.Integration.Tests.Cqrs.Commands;
using SharedKernel.Integration.Tests.Shared;
using Xunit;

namespace SharedKernel.Integration.Tests.Cqrs.Middlewares
{
    public class MiddlewaresValidationsTests : InfrastructureTestCase
    {
        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services
                .AddSharedKernel()
                .AddCommandsHandlers(typeof(SampleCommandHandler))
                .AddValidators(typeof(SampleCommandHandler))
                .AddTransient(typeof(IMiddleware<>), typeof(ValidationMiddleware<>))
                .AddInMemoryCommandBus();
        }

        [Fact]
        public async Task TestCommandHandlerWithResponse()
        {
            const int value = 15;
            var request = new SampleCommand(value);

            var result = await Record.ExceptionAsync(() => GetRequiredService<ISender>().SendAsync(request, CancellationToken.None));

            result.Should().BeNull();
        }

        [Fact]
        public async Task TestCommandHandlerWithResponseValidationError()
        {
            var request = new SampleCommand(default);
            await Assert.ThrowsAsync<ValidationFailureException>(() =>
                GetRequiredService<ISender>().SendAsync(request, CancellationToken.None));
        }
    }
}