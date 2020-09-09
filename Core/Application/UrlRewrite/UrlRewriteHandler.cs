using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCmsLibrary.Core.Application.FileSystem;
using Acmion.CommunicatorCmsLibrary.Core.Application.Pages;
using Acmion.CommunicatorCmsLibrary.Core.Extensions;
using Acmion.CommunicatorCmsLibrary.Core.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Acmion.CommunicatorCmsLibrary.Core.Application.UrlRewrite
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

            var requestedUrl = customRewrittenRequest.Url;
            var requestedQuery = customRewrittenRequest.Query;

            if (!AppUrl.IsDirectlyServable(requestedUrl))
            {
                return;
            }

            if (context.Request.QueryString.Value != requestedQuery)
            {
                context.Request.QueryString = new QueryString(requestedQuery);
            }

            if (AppUrl.IsFile(requestedUrl))
            {
                context.Request.Path = requestedUrl;
            }
            else
            {
                // All urls should end with slash, if not a file
                if (!context.Request.Path.Value.EndsWith(AppUrl.Separator))
                {
                    // Permanent redirect
                    context.Response.StatusCode = 301;
                    context.Response.Redirect(context.Request.Path.Value + AppUrl.Separator);
                    return;
                }

                // Url is an AppPage

                // Get baseUrl and parameters
                var baseUrlAndParameterValues = AppPage.GetBaseUrlAndParameterValues(requestedUrl);
                var actualAppPageUrl = AppPage.GetActualAppPageUrl(baseUrlAndParameterValues);

                var currentAppPage = await App.Pages.GetByUrl(actualAppPageUrl);

                // Store the state requestState
                await requestState.Initialize(requestedUrl, baseUrlAndParameterValues.ParameterValues, currentAppPage);

                // Get rewrite to actualAppPageUrl
                context.Request.Path = actualAppPageUrl + AppPageSettings.IndexUrl;
            }

            await _next(context);
        }
    }
}
