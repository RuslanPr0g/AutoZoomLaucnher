using LaunchService.Launcher;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ScreenRecorderLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LaunchService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMeetingLauncher _launcher;

        public Worker(ILogger<Worker> logger,
            IMeetingLauncher launcher)
        {
            _logger = logger;
            _launcher = launcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(4 * 1000, stoppingToken);
            _logger.LogInformation("Starting the scheduled meeeting...");
            await Task.Delay(1 * 1000, stoppingToken);
            _logger.LogInformation("Launching link in the browser...");

            _launcher.Launch("https://us04web.zoom.us/j/74772721215?pwd=U0t0b1JvSDZZZVZ4alh2RHZXNXYzZz09");

            _logger.LogInformation("Meeting launched successfully!");

            // record video

            _logger.LogInformation("Starting the recording of the meeeting...");
            await Task.Delay(3 * 1000, stoppingToken);
            CreateRecording();
            _logger.LogInformation("Meeting recording successfully started!");
            await Task.Delay(60 * 1000, stoppingToken);
            EndRecording();
            _logger.LogInformation("Meeting recording successfully finished!");
        }

        /// <summary>
        /// Recorder
        /// </summary>

        Recorder _rec;
        Stream _outStream;
        void CreateRecording()
        {
            string videoPath = Path.Combine("C:\\DISKD\\", "test.mp4");
            _rec = Recorder.CreateRecorder();
            _rec.OnRecordingComplete += Rec_OnRecordingComplete;
            _rec.OnRecordingFailed += Rec_OnRecordingFailed;
            _rec.OnStatusChanged += Rec_OnStatusChanged;
            //Record to a file
            //videoPath = Path.Combine(Path.GetTempPath(), "test.mp4");
            _rec.Record(videoPath);
            //..Or to a stream
            //_outStream = new MemoryStream();
            //_rec.Record(_outStream);
        }
        void EndRecording()
        {
            _rec.Stop();
        }
        private void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            //Get the file path if recorded to a file
            string path = e.FilePath;
            //or do something with your stream
            //... something ...
            _outStream?.Dispose();
        }
        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            string error = e.Error;
            _outStream?.Dispose();
        }
        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            RecorderStatus status = e.Status;
        }
    }
}
