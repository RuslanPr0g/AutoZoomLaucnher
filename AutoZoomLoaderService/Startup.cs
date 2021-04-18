using AutoZoomLoaderService.Launcher;
using AutoZoomLoaderService.Models;
using AutoZoomLoaderService.Recorders;
using AutoZoomLoaderService.Serializing;
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
using System.IO;
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
        public async void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IRecurringJobManager recurringJobManager,
            IMeetingLauncher meetingLauncher,
            ScreenRecorder screenRecorder)
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

            try
            {
                string meetingsConfigurationPath = "C:\\DISKD\\meetingconfiguration.json";
                string weektypesConfigurationPath = "C:\\DISKD\\weektypeconfiguration.json";

                // await Constants.GenerateConfigurationMeetings(meetingsConfigurationPath, weektypesConfigurationPath);

                var jsonMeetings = File.ReadAllText(meetingsConfigurationPath);
                var meetings = new JsonFileSerializer().Deserialize<List<MeetingModel>>(jsonMeetings);

                var jsonWeekTypes = File.ReadAllText(weektypesConfigurationPath);
                var weekTypes = new JsonFileSerializer().Deserialize<List<WeekTypeModel>>(jsonWeekTypes);

                foreach (var meeting in meetings)
                {
                    recurringJobManager.AddOrUpdate(
                        meeting.Id + "_" + meeting.Name,
                        () => new RecordMeeting(meetingLauncher, screenRecorder)
                            .Execute(meeting, weekTypes, CancellationToken.None),
                        meeting.CronSchedule
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
