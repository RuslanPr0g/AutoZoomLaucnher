using LaunchService.Launcher;
using LaunchService.Models;
using LaunchService.Recorders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LaunchService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMeetingLauncher _launcher;
        private readonly ScreenRecorder _screenRecorder;

        public Worker(ILogger<Worker> logger,
            IMeetingLauncher launcher, ScreenRecorder screenRecorder)
        {
            _logger = logger;
            _launcher = launcher;
            _screenRecorder = screenRecorder;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // configure the jobs

            // ...




            // launching the meeting

            await Task.Delay(2 * 1000, stoppingToken);

            Console.WriteLine("Enter a zoom link: ");
            string ZOOM_MEETING_LINK = Console.ReadLine();

            Console.WriteLine("Enter meeting name (without an extention): ");
            string meetingName = DateTime.Now.Date.Year + "-" + DateTime.Now.Date.Month + "-" 
                + DateTime.Now.Date.Day + "_" + Console.ReadLine() + ".mp4";

            Console.WriteLine("Enter duration of the video (in seconds): ");
            int secondsToRecord = Convert.ToInt32(Console.ReadLine());

            const string basePath = "C:\\DISKD\\";

            _logger.LogInformation("Starting the scheduled meeeting...");
            await Task.Delay(1 * 1000, stoppingToken);
            _logger.LogInformation("Launching link in the browser...");

            _launcher.Launch(ZOOM_MEETING_LINK);
            _logger.LogInformation("Meeting launched successfully!");

            // recording the video

            _logger.LogInformation("Starting the recording of the meeeting...");
            await Task.Delay(3 * 1000, stoppingToken);
            _screenRecorder.CreateRecording(basePath, meetingName);
            _logger.LogInformation("Meeting recording successfully started!");
            await Task.Delay(secondsToRecord * 1000, stoppingToken);
            _screenRecorder.EndRecording();
            _logger.LogInformation("Meeting recording successfully finished!");
        }
    }
}
