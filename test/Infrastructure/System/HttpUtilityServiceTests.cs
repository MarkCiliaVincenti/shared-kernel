﻿using SharedKernel.Infrastructure.System;
using Xunit;

namespace SharedKernel.Integration.Tests.System
{
    public class HttpUtilityServiceTests
    {
        [Fact]
        public void HttpUtilityService()
        {
            var x = new WebUtils().HtmlEncode("/");

            Assert.NotNull(x);
            Assert.Equal("/", x);
        }
    }
}
