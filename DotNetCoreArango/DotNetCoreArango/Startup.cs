using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetCoreArango.Services;
using BorderEast.ArangoDB.Client;
using System;
using DotNetCoreArango.Models;
using BorderEast.ASPNetCore.Identity.ArangoDB;
using DotNetCoreArango.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace DotNetCoreArango
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            ArangoClient.Client().SetDefaultDatabase(new BorderEast.ArangoDB.Client.Database.ClientSettings()
            {
                DatabaseName = "_system",
                Protocol = BorderEast.ArangoDB.Client.Database.ProtocolType.HTTP,
                ServerAddress = "localhost",
                ServerPort = 8529,
                SystemCredential = new System.Net.NetworkCredential("root", ""),
                DatabaseCredential = new System.Net.NetworkCredential("root", ""),
                AutoCreate = true,
                HTTPClient = new System.Net.Http.HttpClient(),
                IsDebug = true
            });

            services.AddSingleton<IArangoClient>(ArangoClient.Client()); // AddSingleton shared with everyone who needs it for lifetime of the application
            services.AddScoped<IContactStore, ArangoContactStore>(); // AddScoped adds it within the scope of a single request
            services.AddAutoMapper(); // for doing mappings/custom mappings (resolvers) between Entities and models
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // for ContactUrlResolver
            services.AddCors();

            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Cookies.ApplicationCookie.AuthenticationScheme = "ApplicationCookie";
                options.Cookies.ApplicationCookie.CookieName = "Interop";
            })
            .AddArangoDbStores()
            .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>(); // AddTransient recreates it everytime
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCors(cfg =>
            {
                cfg.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
            });

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
