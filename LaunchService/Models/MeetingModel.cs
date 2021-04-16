using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchService.Models
{
    public class MeetingModel
    {
        public int Id { get; set; }
        public string MeetingLink { get; set; }
        public string Name { get; set; }
        public string CronSchedule { get; set; }
        public string WeekType { get; set; }
        public int VideoDuration { get; set; } = 4860;
    }
}
