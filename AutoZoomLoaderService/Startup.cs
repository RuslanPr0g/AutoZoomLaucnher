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

                // await GenerateConfigurationMeetings(meetingsConfigurationPath, weektypesConfigurationPath);

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

        private async Task GenerateConfigurationMeetings(string meetingConfigurationPath, string weektypesConfigurationPath)
        {
            var meetings = new List<MeetingModel>() {
                new MeetingModel()
                {
                    Id = 0,
                    MeetingLink = "https://us04web.zoom.us/j/76380148912?pwd=RS96aUEvMlpHK29EenQydE1pRmUwZz09",
                    Name = "TestMeeting1",
                    CronSchedule = "* * * * *",
                    WeekType = "Even",
                    VideoDuration = 4900
                }
            };

            var weekTypes = new List<WeekTypeModel>()
            {
                new WeekTypeModel()
                {
                    WeekDateStart = DateTime.Parse("4/12/2021"),
                    WeekDateEnd = DateTime.Parse("4/19/2021"),
                    Type = "Even"
                },
                new WeekTypeModel()
                {
                    WeekDateStart = DateTime.Parse("4/19/2021"),
                    WeekDateEnd = DateTime.Parse("4/26/2021"),
                    Type = "Odd"
                },
                new WeekTypeModel()
                {
                    WeekDateStart = DateTime.Parse("4/26/2021"),
                    WeekDateEnd = DateTime.Parse("5/3/2021"),
                    Type = "Even"
                },
                new WeekTypeModel()
                {
                    WeekDateStart = DateTime.Parse("5/3/2021"),
                    WeekDateEnd = DateTime.Parse("5/10/2021"),
                    Type = "Odd"
                },
                new WeekTypeModel()
                {
                    WeekDateStart = DateTime.Parse("5/10/2021"),
                    WeekDateEnd = DateTime.Parse("5/17/2021"),
                    Type = "Even"
                },
                new WeekTypeModel()
                {
                    WeekDateStart = DateTime.Parse("4/19/2021"),
                    WeekDateEnd = DateTime.Parse("4/26/2021"),
                    Type = "Odd"
                },
                new WeekTypeModel()
                {
                    WeekDateStart = DateTime.Parse("5/17/2021"),
                    WeekDateEnd = DateTime.Parse("5/24/2021"),
                    Type = "Even"
                },
                new WeekTypeModel()
                {
                    WeekDateStart = DateTime.Parse("5/24/2021"),
                    WeekDateEnd = DateTime.Parse("5/31/2021"),
                    Type = "Odd"
                },
                new WeekTypeModel()
                {
                    WeekDateStart = DateTime.Parse("5/31/2021"),
                    WeekDateEnd = DateTime.Parse("6/7/2021"),
                    Type = "Even"
                },
            };

            var jsonFileButesWithMeetings = await new JsonFileSerializer().Serialize(meetings);
            var jsonFileButesWithWeekTypes = await new JsonFileSerializer().Serialize(weekTypes);

            File.WriteAllBytes(meetingConfigurationPath, jsonFileButesWithMeetings);
            File.WriteAllBytes(weektypesConfigurationPath, jsonFileButesWithWeekTypes);
        }
    }
}
