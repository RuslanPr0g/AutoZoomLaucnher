using AutoZoomLoaderService.Launcher;
using AutoZoomLoaderService.Models;
using AutoZoomLoaderService.Recorders;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoZoomLoaderService
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
            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage());

            services.AddHangfireServer();

            services.AddTransient<IMeetingLauncher, MeetingLauncher>();
            services.AddTransient<ScreenRecorder>();
            services.AddTransient<RecordMeeting>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHangfireDashboard();
            backgroundJobClient.Enqueue(() =>
                Console.WriteLine("Good, good, bu' could be better than tha'"));

            // Configure jobs for the meetings
            // 1) read the config file with meetings
            // 2) deserialize them into the list of meeting models
            // 3) using loop statement go through all of the meetings and create job for each of them



            recurringJobManager.AddOrUpdate(
                    "Run every minute",
                    () => Console.WriteLine("That's bettter dude!"),
                    "* * * * *"
                );
        }
    }
}
