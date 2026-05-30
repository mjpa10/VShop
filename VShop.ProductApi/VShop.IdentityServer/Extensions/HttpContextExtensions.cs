using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Duende.IdentityServer.Extensions
{
    internal static class HttpContextExtensions
    {
        public static async Task<bool> GetSchemeSupportsSignOutAsync(this HttpContext context, string scheme)
        {
            if (string.IsNullOrEmpty(scheme) || context == null) return false;

            var schemeProvider = context.RequestServices.GetService<IAuthenticationSchemeProvider>();
            if (schemeProvider == null) return false;

            var authScheme = await schemeProvider.GetSchemeAsync(scheme);
            if (authScheme == null) return false;

            // Consider a scheme to support sign-out if it has a display name (external provider)
            // or if the handler type indicates an OpenID Connect / WS-Fed style handler.
            if (!string.IsNullOrEmpty(authScheme.DisplayName)) return true;

            var handlerType = authScheme.HandlerType?.Name ?? string.Empty;
            if (handlerType.Contains("OpenIdConnect") || handlerType.Contains("WsFederation") || handlerType.Contains("OAuth"))
            {
                return true;
            }

            return false;
        }
    }
}
