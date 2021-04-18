using AutoZoomLoaderService.Launcher;
using AutoZoomLoaderService.Models;
using AutoZoomLoaderService.Recorders;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoZoomLoaderService
{
    public class RecordMeeting
    {
        private readonly IMeetingLauncher _launcher;
        private readonly ScreenRecorder _screenRecorder;

        public RecordMeeting(IMeetingLauncher launcher,
            ScreenRecorder screenRecorder)
        {
            _launcher = launcher;
            _screenRecorder = screenRecorder;
        }

        public async Task Execute(MeetingModel meetingModel, List<WeekTypeModel> weekTypes,
            CancellationToken stoppingToken)
        {
            if(meetingModel.WeekType != "Any")
            {
                var currentWeekString = DateTime.Now.Date.Month + "/" + DateTime.Now.Date.Day + "/" + DateTime.Now.Date.Year;
                var currentWeekDate = DateTime.Parse(currentWeekString);
                string currentWeekType = weekTypes.First(week => 
                    currentWeekDate.Date >= week.WeekDateStart.Date &&
                    currentWeekDate.Date < week.WeekDateEnd.Date).Type;

                if (meetingModel.WeekType != currentWeekType)
                {
                    Console.WriteLine("Skip this meeting, week type is different, yahoo!");
                    return;
                }
            }

            // launching the meeting

            string ZOOM_MEETING_LINK = meetingModel.MeetingLink;
            Console.WriteLine("Zoom link: " + ZOOM_MEETING_LINK);

            string meetingFileName = DateTime.Now.Date.Year + "-" + DateTime.Now.Date.Month + "-"
                + DateTime.Now.Date.Day + "_" + new Random().Next(4983) + "_" + meetingModel.Name + ".mp4";
            Console.WriteLine("Meeting file name: " + meetingFileName);

            //int secondsToRecord = Convert.ToInt32(meetingModel.VideoDuration);
            const int meetingMinutesLimit = 42;
            //const int meetingMinutesLimit = 2;
            int secondsToRecord = 60 * meetingMinutesLimit;
            Console.WriteLine("Duration of the video (in minutes): " + meetingMinutesLimit * 2);

            const string basePath = "D:\\videoszoommeetings\\";

            Console.WriteLine("Starting the scheduled meeeting...");
            await Task.Delay(1 * 1000, stoppingToken);
            Console.WriteLine("Launching link in the browser...");

            _launcher.Launch(ZOOM_MEETING_LINK);
            Console.WriteLine("Meeting the first part launched successfully!");

            // recording the video

            Console.WriteLine("Starting the recording of the meeeting...");
            await Task.Delay(3 * 1000, stoppingToken);
            _screenRecorder.CreateRecording(basePath, meetingFileName);
            Console.WriteLine("Meeting recording successfully started!");
            await Task.Delay(secondsToRecord * 1000, stoppingToken);

            Console.WriteLine("Meeting finished, rejoining after 1 minutes...");
            await Task.Delay(60 * 1 * 1000, stoppingToken);
            Console.WriteLine("Rejoining...");
            _launcher.Launch(ZOOM_MEETING_LINK);
            Console.WriteLine("Meeting the second part launched successfully!");
            await Task.Delay(secondsToRecord * 1000, stoppingToken);
            _screenRecorder.EndRecording();

            Console.WriteLine("Meeting recording successfully finished! You can find recording here: " + basePath + meetingFileName);
        }
    }
}
