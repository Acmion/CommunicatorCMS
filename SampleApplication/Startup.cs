using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms;
using Acmion.CommunicatorCms.Core;
using Acmion.CommunicatorCms.Core.Application;
using Acmion.CommunicatorCms.Core.Application.Auhtorization;
using Acmion.CommunicatorCms.Core.Application.StaticFiles;
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

            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddScoped<RequestState>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            CommunicatorCmsConfiguration.Main(Program.RootPath);

            app.UseMiddleware<UrlRewriteHandler>();

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
            app.UseStaticFiles(new StaticFileHandler(env.IsDevelopment() ? TimeSpan.FromSeconds(10) : TimeSpan.FromDays(1), true));

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<AuthorizationHandler>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
