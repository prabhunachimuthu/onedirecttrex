using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneDirect.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor;
using OneDirect.Helper;
using Serilog;
using System.IO;
using Serilog.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneDirect.ViewModels;
using System.Collections.Specialized;

namespace OneDirect
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "Logs\\log-{Date}.txt"), LogEventLevel.Debug,
                   outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}").
               CreateLogger();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            TimeSlot timeslot = new TimeSlot();
            Configuration.GetSection("TimeSlot").Bind(timeslot);
            ConfigVars.NewInstance.slots = timeslot;

            //ConfigVars.NewInstance.slots = Configuration.GetSection("TimeSlot").Get<TimeSlot>();

            services.AddDbContext<OneDirectContext>(options =>
 options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.CookieHttpOnly = true;
            });
            //services.AddMvc().AddJsonOptions(options => {
            //    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //});
            services.AddScoped<IRequestFactory, RequestFactory>();

            //services.Configure((RazorViewEngineOptions options) =>
            //{
            //    var previous = options.CompilationCallback;
            //    options.CompilationCallback = (context) =>
            //    {
            //        previous?.Invoke(context);
            //        context.Compilation = context.Compilation.AddReferences(OneDirect);
            //    };
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                   template: "{controller=Login}/{action=Index}/{id?}");

                routes.MapRoute(
                     name: "DefaultApi",
                     template: "api/{controller}/{id}");
                routes.MapRoute(
                   name: "DefaultApiWithAction",
                   template: "api/{controller}/{action}/{id}");

            });

            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }


        //public void Configure(IApplicationBuilder app)
        //{
        //    app.UseSession();
        //    app.UseMvcWithDefaultRoute();
        //}
    }
}
