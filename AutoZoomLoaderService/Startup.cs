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

                 //await GenerateConfigurationMeetings(meetingsConfigurationPath, weektypesConfigurationPath);

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
                    MeetingLink = "",
                    Name = "ComputerGraphics",
                    CronSchedule = "59 4 * * 1",
                    WeekType = "Even",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 1,
                    MeetingLink = "",
                    Name = "PKI",
                    CronSchedule = "59 4 * * 1",
                    WeekType = "Odd",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 2,
                    MeetingLink = "",
                    Name = "English",
                    CronSchedule = "44 6 * * 1",
                    WeekType = "Any",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 3,
                    MeetingLink = "",
                    Name = "CompGraphics",
                    CronSchedule = "29 8 * * 1",
                    WeekType = "Even",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 4,
                    MeetingLink = "",
                    Name = "CompNetworks",
                    CronSchedule = "29 8 * * 1",
                    WeekType = "Odd",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 5,
                    MeetingLink = "",
                    Name = "PKIoddMonday",
                    CronSchedule = "14 8 * * 1",
                    WeekType = "Odd",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 6,
                    MeetingLink = "",
                    Name = "CPLUSPLUS",
                    CronSchedule = "59 4 * * 2",
                    WeekType = "Any",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 7,
                    MeetingLink = "",
                    Name = "OS",
                    CronSchedule = "29 8 * * 2",
                    WeekType = "Even",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 8,
                    MeetingLink = "",
                    Name = "CompNetworks",
                    CronSchedule = "14 10 * * 2",
                    WeekType = "Even",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 9,
                    MeetingLink = "",
                    Name = "Fizra",
                    CronSchedule = "59 4 * * 3",
                    WeekType = "None",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 10,
                    MeetingLink = "",
                    Name = "HTMLCSSJS",
                    CronSchedule = "59 4 * * 4",
                    WeekType = "Any",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 11,
                    MeetingLink = "",
                    Name = "NumericalMethods",
                    CronSchedule = "29 8 * * 4",
                    WeekType = "Any",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 12,
                    MeetingLink = "",
                    Name = "Politology",
                    CronSchedule = "59 5 * * 5",
                    WeekType = "Any",
                    VideoDuration = 4900
                },
                new MeetingModel()
                {
                    Id = 13,
                    MeetingLink = "",
                    Name = "OSFriday",
                    CronSchedule = "29 8 * * 5",
                    WeekType = "Any",
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
