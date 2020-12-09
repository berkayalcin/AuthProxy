using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace AuthProxy
{
    public static class ClaimsIdentityExtensions
    {
        public static void RegisterPermissions(this ClaimsIdentity claimsIdentity, params string[] permissions)
        {
            if (permissions == null || permissions.Length == 0)
                return;

            var proxyPermissions = permissions.Select(x => new Permission()
            {
                Id = Guid.NewGuid(),
                Name = x
            });

            claimsIdentity.AddClaim(new Claim(Constants.AuthProxyPermissionClaimType,
                JsonSerializer.Serialize(proxyPermissions)));
        }
    }
}