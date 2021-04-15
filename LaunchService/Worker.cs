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
            // launch the meeting

            await Task.Delay(4 * 1000, stoppingToken);
            _logger.LogInformation("Starting the scheduled meeeting...");
            await Task.Delay(1 * 1000, stoppingToken);
            _logger.LogInformation("Launching link in the browser...");

            string ZOOM_MEETING_LINK = "https://us04web.zoom.us/j/71470382288?pwd=UWdTZFdWRkR4bE11M1JMVkJqY2ZlUT09";
            string basePath = "C:\\DISKD\\";
            string videoName = "meeting.mp4";

            _launcher.Launch(ZOOM_MEETING_LINK);

            _logger.LogInformation("Meeting launched successfully!");

            // record the video

            _logger.LogInformation("Starting the recording of the meeeting...");
            await Task.Delay(3 * 1000, stoppingToken);
            CreateRecording(basePath, videoName); 
            _logger.LogInformation("Meeting recording successfully started!");
            await Task.Delay(30 * 1000, stoppingToken);
            EndRecording();
            _logger.LogInformation("Meeting recording successfully finished!");
        }

        /// <summary>
        /// Video Recorder
        /// </summary>

        Recorder _rec;
        Stream _outStream;
        void CreateRecording(string basePath, string videoName)
        {
            string videoPath = Path.Combine(basePath, videoName);

            var inputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices);
            var outputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices);
            string selectedOutputDevice = null;
            string selectedInputDevice = null;

            RecorderOptions options = new RecorderOptions
            {
                RecorderMode = RecorderMode.Video,
                //If throttling is disabled, out of memory exceptions may eventually crash the program,
                //depending on encoder settings and system specifications.
                IsThrottlingDisabled = false,
                //Hardware encoding is enabled by default.
                IsHardwareEncodingEnabled = true,
                //Low latency mode provides faster encoding, but can reduce quality.
                IsLowLatencyEnabled = false,
                //Fast start writes the mp4 header at the beginning of the file, to facilitate streaming.
                IsMp4FastStartEnabled = false,
                AudioOptions = new AudioOptions
                {
                    Bitrate = AudioBitrate.bitrate_128kbps,
                    Channels = AudioChannels.Stereo,
                    IsAudioEnabled = true,
                    IsOutputDeviceEnabled = true,
                    IsInputDeviceEnabled = true,
                    AudioOutputDevice = selectedOutputDevice,
                    AudioInputDevice = selectedInputDevice
                },
                VideoOptions = new VideoOptions
                {
                    BitrateMode = BitrateControlMode.UnconstrainedVBR,
                    Bitrate = 8000 * 1000,
                    Framerate = 60,
                    IsFixedFramerate = true,
                    EncoderProfile = H264Profile.Main
                },
                MouseOptions = new MouseOptions
                {
                    //Displays a colored dot under the mouse cursor when the left mouse button is pressed.	
                    IsMouseClicksDetected = true,
                    MouseClickDetectionColor = "#FFFF00",
                    MouseRightClickDetectionColor = "#FFFF00",
                    MouseClickDetectionRadius = 30,
                    MouseClickDetectionDuration = 100,

                    IsMousePointerEnabled = true,
                    /* Polling checks every millisecond if a mouse button is pressed.
                       Hook works better with programmatically generated mouse clicks, but may affect
                       mouse performance and interferes with debugging.*/
                    MouseClickDetectionMode = MouseDetectionMode.Hook
                }
            };
            _rec = Recorder.CreateRecorder(options);
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
