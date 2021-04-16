using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoZoomLoaderService.Models
{
    public class WeekTypeModel
    {
        public DateTime WeekDateStart { get; set; }
        public DateTime WeekDateEnd { get; set; }
        public string Type { get; set; }
    }
}
