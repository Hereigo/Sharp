using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DotNet8.Models
{
    public class CalEventVM
    {
        public CalEventVM() { }

        public CalEventVM(CalEvent calEvent)
        {
            var time = calEvent.Started.ToString("HH:mm");

            Id = calEvent.Id;
            DayOfMonth = calEvent.Started.Day;
            Description = calEvent.Description;
            Time = time == "00:00" ? string.Empty : time;
        }

        public CalEventCategory Category { get; set; }
        public CalEventRepeat Repeat { get; set; }
        public CalEventStatus Status { get; set; }

        public DateTime Modified { get; set; }

        //[DisplayFormat(DataFormatString="{0:D}")]
        //[DisplayFormat(ApplyFormatInEditMode=true, DataFormatString = "{0:yyyy.MM.dd}")]
        public DateTime Started { get; set; }

        public IEnumerable<SelectListItem> RepeatList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }

        public int Day { get; set; }
        public int DayOfMonth;
        public int Month { get; set; }
        public int Year { get; set; }
        public int? EveryXDays { get; set; }
        public int? Id;

        public string Description;
        public string Time;
    }
}
