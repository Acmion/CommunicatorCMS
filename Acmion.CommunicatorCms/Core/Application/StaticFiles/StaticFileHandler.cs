using Acmion.CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Application.StaticFiles
{
    public class StaticFileHandler : StaticFileOptions
    {
        public StaticFileHandler(TimeSpan maxAge, bool serveUnknownFileTypes)
        {
            FileProvider = new PhysicalFileProvider(CommunicatorCmsConfiguration.AppAbsolutePath + GeneralSettings.RazorPagesRootAppPath);
            RequestPath = "";
            OnPrepareResponse = ctx =>
            {
                var headers = ctx.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = maxAge
                };
            };
            ServeUnknownFileTypes = serveUnknownFileTypes; // Security risk according to https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1, but unlikely
        }
    }
}
