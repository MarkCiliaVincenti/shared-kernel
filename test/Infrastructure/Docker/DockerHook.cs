﻿using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using System;
using Xunit;

namespace SharedKernel.Integration.Tests.Docker
{
    [CollectionDefinition("DockerHook")]
    public class DockerHookCollection : ICollectionFixture<DockerHook>, IDisposable
    {
        private readonly DockerHook _dockerHook;

        public DockerHookCollection(DockerHook dockerHook)
        {
            _dockerHook = dockerHook;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _dockerHook?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class DockerHook : IDisposable
    {
        private readonly ICompositeService _compositeService;

        public DockerHook()
        {
            _compositeService = new Builder()
                .UseContainer()
                .UseCompose()
                .FromFile("./docker-compose.yml")
                .RemoveOrphans()
                .Build()
                .Start();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _compositeService.Stop();
            _compositeService?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
