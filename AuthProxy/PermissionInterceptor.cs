using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;

namespace AuthProxy
{
    public sealed class PermissionInterceptor : IPermissionInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodAuthorizeAttribute =
                invocation.MethodInvocationTarget.GetCustomAttribute(typeof(AuthorizeAttribute), false) as
                    AuthorizeAttribute;
            var classAuthorizeAttribute =
                invocation.Method.DeclaringType?.GetCustomAttribute(typeof(AuthorizeAttribute), false) as
                    AuthorizeAttribute;

            if (methodAuthorizeAttribute == null && classAuthorizeAttribute == null)
            {
                invocation.Proceed();
                return;
            }

            var usersClaims = _httpContextAccessor
                .HttpContext
                .User
                .Claims
                .FirstOrDefault(c => c.Type == Constants.AuthProxyPermissionClaimType)?
                .Value;

            if (string.IsNullOrEmpty(usersClaims))
            {
                throw new UnauthorizedAccessException(
                    $"Unauthorized Access to {invocation.MethodInvocationTarget.Name} with " +
                    $"{string.Join(",", invocation.Arguments)}");
            }

            var userPermissions = JsonSerializer.Deserialize<List<Permission>>(usersClaims);


            CheckAuthorization(invocation, classAuthorizeAttribute, userPermissions);
            CheckAuthorization(invocation, methodAuthorizeAttribute, userPermissions);

            invocation.Proceed();
        }

        private static void CheckAuthorization(IInvocation invocation, AuthorizeAttribute authorizeAttribute,
            List<Permission> userPermissions)
        {
            if (authorizeAttribute == null)
                return;

            var classPermissions = authorizeAttribute.Permissions;
            var hasRequiredPermissions = classPermissions
                .All(cp => userPermissions.Any(p => p.Name.Equals(cp)));

            if (!hasRequiredPermissions)
                throw new UnauthorizedAccessException(
                    $"Unauthorized Access to {invocation.MethodInvocationTarget.Name} with " +
                    $"{string.Join(",", invocation.Arguments)}");
        }
    }
}