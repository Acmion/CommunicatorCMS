using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application.FileSystem;
using Acmion.CommunicatorCms.Core.Application.Pages;
using Acmion.CommunicatorCms.Core.Extensions;
using Acmion.CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Acmion.CommunicatorCms.Core.Application.UrlRewrite
{ 
    public class UrlRewriteHandler
    {
        private readonly RequestDelegate _next;

        public UrlRewriteHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, RequestState requestState)
        {
            var customRewrittenRequest = App.UrlRewriteCustom.Rewrite(context.Request.Path.Value, context.Request.QueryString.Value);

            var requestedUrl = AppUrl.ConvertAspNetCoreUrlToActualUrl(customRewrittenRequest.Url);
            var requestedQuery = customRewrittenRequest.Query;

            // Get baseUrl and parameters
            var baseUrlAndParameterValues = AppPage.GetBaseUrlAndParameterValues(requestedUrl);
            var actualAppPageUrl = AppPage.GetActualAppPageUrl(baseUrlAndParameterValues);

            if (!AppUrl.IsDirectlyServable(requestedUrl))
            {
                return;
            }

            context.Request.Path = new PathString(requestedUrl);

            if (context.Request.QueryString.Value != requestedQuery)
            {
                context.Request.QueryString = new QueryString(requestedQuery);
            }

            if (AppPage.IsUrlAppPage(actualAppPageUrl))
            {
                // All urls should end with slash, if not a file
                if (!context.Request.Path.Value.EndsWith(AppUrl.Separator))
                {
                    // Permanent redirect
                    context.Response.StatusCode = 301;
                    context.Response.Redirect(context.Request.Path.Value + AppUrl.Separator + requestedQuery);
                    return;
                }

                var currentAppPage = await App.Pages.GetByUrl(actualAppPageUrl);

                // Store the state requestState
                await requestState.Initialize(requestedUrl, requestedQuery, baseUrlAndParameterValues.ParameterValues, currentAppPage);

                // Get rewrite to actualAppPageUrl
                context.Request.Path = actualAppPageUrl + AppPageSettings.IndexUrl;
            }

            await _next(context);
        }
    }
}
