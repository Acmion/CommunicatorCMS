using Acmion.CommunicatorCms.Core.Application.Pages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Application.Auhtorization
{
    public class AuthorizationHandler
    {
        private readonly RequestDelegate _next;

        public AuthorizationHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, RequestState requestState, IHttpContextAccessor httpContextAccessor)
        {
            var currentAppPage = requestState.CurrentAppPage;
            var requestedUrl = requestState.Url;
            var requestedQuery = requestState.Query;

            if (currentAppPage != null && currentAppPage != AppPage.EmptyAppPage && currentAppPage.Properties.Authorization.Roles.Count > 0)
            {
                var httpContext = httpContextAccessor.HttpContext;

                if (httpContext == null)
                {
                    // Some error, should never happen, but kill just in case.
                    return;
                }

                if (httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
                {
                    // Not signed in. Not permanent redirect
                    context.Response.StatusCode = StatusCodes.Status302Found;
                    context.Response.Redirect(App.Settings.SignInUrl + "?" + App.Settings.ReturnUrlParameter + "=" + WebUtility.UrlEncode(requestedUrl + requestedQuery));
                    return;
                }
                else
                {
                    foreach (var role in currentAppPage.Properties.Authorization.Roles)
                    {
                        if (!httpContext.User.IsInRole(role))
                        {
                            // Not authorized. Not permanent redirect
                            context.Response.StatusCode = StatusCodes.Status302Found;
                            context.Response.Redirect(App.Settings.NotAuthorizedUrl + "?" + App.Settings.ReturnUrlParameter + "=" + WebUtility.UrlEncode(requestedUrl + requestedQuery));
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }

}
