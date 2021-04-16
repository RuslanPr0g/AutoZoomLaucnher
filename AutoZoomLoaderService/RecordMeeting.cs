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
        private readonly ILogger<RecordMeeting> _logger;
        private readonly IMeetingLauncher _launcher;
        private readonly ScreenRecorder _screenRecorder;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public RecordMeeting(ILogger<RecordMeeting> logger,
            IMeetingLauncher launcher, ScreenRecorder screenRecorder,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _launcher = launcher;
            _screenRecorder = screenRecorder;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task Execute(MeetingModel meetingModel,
            CancellationToken stoppingToken)
        {
            //// launching the meeting

            await Task.Delay(2 * 1000, stoppingToken);
            Console.WriteLine("Executed successfully!");

            //Console.WriteLine("Enter a zoom link: ");
            //string ZOOM_MEETING_LINK = Console.ReadLine();

            //Console.WriteLine("Enter meeting name (without an extention): ");
            //string meetingName = DateTime.Now.Date.Year + "-" + DateTime.Now.Date.Month + "-" 
            //    + DateTime.Now.Date.Day + "_" + Console.ReadLine() + ".mp4";

            //Console.WriteLine("Enter duration of the video (in seconds): ");
            //int secondsToRecord = Convert.ToInt32(Console.ReadLine());

            //const string basePath = "C:\\DISKD\\";

            //_logger.LogInformation("Starting the scheduled meeeting...");
            //await Task.Delay(1 * 1000, stoppingToken);
            //_logger.LogInformation("Launching link in the browser...");

            //_launcher.Launch(ZOOM_MEETING_LINK);
            //_logger.LogInformation("Meeting launched successfully!");

            //// recording the video

            //_logger.LogInformation("Starting the recording of the meeeting...");
            //await Task.Delay(3 * 1000, stoppingToken);
            //_screenRecorder.CreateRecording(basePath, meetingName);
            //_logger.LogInformation("Meeting recording successfully started!");
            //await Task.Delay(secondsToRecord * 1000, stoppingToken);
            //_screenRecorder.EndRecording();
            //_logger.LogInformation("Meeting recording successfully finished!");
        }
    }
}
