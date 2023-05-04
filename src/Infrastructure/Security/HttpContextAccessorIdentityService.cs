﻿using Microsoft.AspNetCore.Http;
using SharedKernel.Application.Logging;
using SharedKernel.Domain.Security;
using System;
using System.Security.Claims;

namespace SharedKernel.Infrastructure.Security
{
    /// <summary>
    /// User authentication
    /// </summary>
    public class HttpContextAccessorIdentityService : IIdentityService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        public HttpContextAccessorIdentityService(
            IHttpContextAccessor httpContextAccessor = null,
            ICustomLogger<HttpContextAccessorIdentityService> logger = null)
        {
            if (httpContextAccessor == null)
            {
                logger?.Info("IHttpContextAccessor not registered");
                return;
            }

            User = httpContextAccessor.HttpContext?.User;

            var request = httpContextAccessor.HttpContext?.Request;

            if (request != default)
            {
                BasePath = $"{request.Scheme}://{request.Host}{request.PathBase}";

                UserAgent = request.Headers["User-Agent"].ToString();
            }

            RemoteIpAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }

        /// <summary>
        /// Identifier
        /// </summary>
        public Guid UserId => GetUserId();

        /// <summary>
        /// User
        /// </summary>
        public ClaimsPrincipal User { get; }

        /// <summary>
        /// Contains a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAuthenticated()
        {
            return User?.Identity?.IsAuthenticated == true;
        }

        /// <summary>
        /// 
        /// </summary>
        public string BasePath { get; }

        /// <summary>
        /// Browser
        /// </summary>
        public string UserAgent { get; }

        /// <summary>
        /// Remote ip address
        /// </summary>
        public string RemoteIpAddress { get; }

        /// <summary>
        /// Get user id from <see cref="ClaimTypes.Sid"/>
        /// </summary>
        /// <returns></returns>
        protected virtual Guid GetUserId()
        {
            var id = User?.FindFirst(ClaimTypes.Sid)?.Value;

            return !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out _) ? new Guid(id) : Guid.Empty;
        }
    }
}
