using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms;
using Acmion.CommunicatorCms.Core;
using Acmion.CommunicatorCms.Core.Application;
using Acmion.CommunicatorCms.Core.Application.UrlRewrite;
using Acmion.CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace SampleApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
            {
                opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                opt.SlidingExpiration = true;
                opt.ExpireTimeSpan = new TimeSpan(24, 0, 0);
                opt.AccessDeniedPath = App.Settings.NotAuthorizedUrl;
                opt.ReturnUrlParameter = App.Settings.ReturnUrlParameter;
                opt.LoginPath = App.Settings.SignInUrl;
            });

            services.AddControllers();

            services.AddRazorPages().AddRazorRuntimeCompilation().AddRazorRuntimeCompilation(options =>
            {
                var libraryPath = Path.GetFullPath(Path.Combine(Program.RootPath, "..", "Acmion.CommunicatorCms"));
                options.FileProviders.Add(new PhysicalFileProvider(libraryPath));
            });

            services.AddHttpContextAccessor();

            ConfigureCommunicatorCmsServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            ConfigureCommunicatorCms(app, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }

        public void ConfigureCommunicatorCmsServices(IServiceCollection services) 
        {
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddScoped<RequestState>();
        }

        public void ConfigureCommunicatorCms(IApplicationBuilder app, IWebHostEnvironment env) 
        {
            CommunicatorCmsConfiguration.Main(Program.RootPath);

            app.UseMiddleware<UrlRewriteHandler>();

            var staticFileCacheTimeSpan = TimeSpan.FromDays(1);

            if (env.IsDevelopment())
            {
                staticFileCacheTimeSpan = TimeSpan.FromMinutes(1);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Join(Program.RootPath, GeneralSettings.RazorPagesRootAppPath)),
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
    }
}
