using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Acmion.CommunicatorCms.Core;
using Acmion.CommunicatorCms.Core.Application;
using Acmion.CommunicatorCms.Core.Application.UrlRewrite;
using Acmion.CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace Acmion.CommunicatorCms
{
    public static class CommunicatorCmsConfiguration
    {
        public static string AppAbsolutePath { get; private set; } = "";
        public static string CmsAbsolutePath { get; } = GetRootPath();

        public static CultureInfo AmericanCultureInfo { get; } = CultureInfo.GetCultureInfo("en-US");

        static CommunicatorCmsConfiguration() 
        {

        }

        public static void Main(string appAbsoluteRootPath) 
        {
            AppAbsolutePath = appAbsoluteRootPath;
        }

        public static IHostBuilder ConfigureCommunicatorCmsWebHost(this IHostBuilder host)
        {
            host.ConfigureWebHostDefaults(webBuilder =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    webBuilder.UseUrls("http://localhost:" + App.Settings.LinuxLocalHostPort)
                              .UseKestrel();
                }
            });
            return host;
        }
        public static IServiceCollection ConfigureCommunicatorCmsServices(this IServiceCollection services) 
        {
            services.AddHttpClient();

            services.AddHttpContextAccessor();

            services.AddScoped<RequestState>();

            return services;
        }
        
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {
            app.UseMiddleware<UrlRewriteHandler>();

            var staticFileCacheTimeSpan = TimeSpan.FromDays(1);

            if (env.IsDevelopment())
            {
                staticFileCacheTimeSpan = TimeSpan.FromMinutes(1);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Join(AppAbsolutePath, GeneralSettings.RazorPagesRootAppPath)),
                RequestPath = "",
                OnPrepareResponse = ctx =>
                {
                    var headers = ctx.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = staticFileCacheTimeSpan
                    };
                },
                ServeUnknownFileTypes = true, // Security risk according to https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1, but unlikely
            });
        }

        private static string GetRootPath([CallerFilePath] string sourceFilePath = "")
        {
            return Path.GetDirectoryName(sourceFilePath)!.Replace('\\', '/');
        }
    }
}
