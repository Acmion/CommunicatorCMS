using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunicatorCms.Core.Application.FileSystem;
using CommunicatorCms.Core.Extensions;
using CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CommunicatorCms.Core.Application.UrlRewrite
{ 
    public class UrlRewriteHandler
    {
        public IUrlRewriter UrlRewriteCustom { get; set; } = new UrlRewriterNone();

        public void UseUrlRewrites(IApplicationBuilder app) 
        {
            app.Use(async (context, next) =>
            {
                var customRewrittenRequest = UrlRewriteCustom.Rewrite(context.Request.Path.Value, context.Request.QueryString.Value);

                var requestedUrl = customRewrittenRequest.Url;
                var requestedQuery = customRewrittenRequest.Query;
                var requestedVirtualUrl = AppUrl.ConvertToVirtualUrl(requestedUrl);
                var requestedActualUrl = AppUrl.ConvertToActualUrl(requestedUrl);

                if (!AppUrl.IsDirectlyServable(requestedActualUrl)) 
                {
                    return;
                }

                if (context.Request.QueryString.Value != requestedQuery) 
                {
                    context.Request.QueryString = new QueryString(requestedQuery);
                }

                if (requestedUrl != requestedVirtualUrl) 
                {
                    // Permanent redirect
                    context.Response.StatusCode = 301;
                    context.Response.Redirect(requestedVirtualUrl);
                    return;
                }

                if (AppUrl.IsFile(requestedActualUrl))
                {
                    context.Request.Path = requestedActualUrl;
                }
                else
                {
                    if (!requestedVirtualUrl.EndsWith(AppUrl.Separator)) 
                    {
                        // Permanent redirect
                        context.Response.StatusCode = 301;
                        context.Response.Redirect(requestedVirtualUrl + AppUrl.Separator);
                        return;
                    }

                    context.Request.Path = requestedActualUrl + AppPageSettings.IndexUrl;
                }

                await next();
            });
        }
    }
}
