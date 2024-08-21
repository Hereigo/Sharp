using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DotNet8.Models
{
    public class CalEventVM
    {
        public CalEventVM() { }

        public CalEventVM(CalEvent calEvent)
        {
            Id = calEvent.Id;
            DayOfMonth = calEvent.Started.Day;
            Description = calEvent.Description;
            Started = calEvent.Started;
            Time = calEvent.Time;
        }

        public CalEventCategory Category { get; set; }
        public CalEventRepeat Repeat { get; set; }
        public CalEventStatus Status { get; set; }

        //[DisplayFormat(DataFormatString="{0:D}")]
        //[DisplayFormat(ApplyFormatInEditMode=true, DataFormatString = "{0:yyyy.MM.dd}")]

        //[DisplayFormat(DataFormatString="{0:yyyy.MM.dd}")]
        [DataType(DataType.Date)]
        public DateTime Started { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString="HH:mm")]
        public TimeSpan Time { get; set; }

        public DateTime Modified { get; set; }

        public IEnumerable<SelectListItem> RepeatList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }

        public int Day { get; set; }
        public int DayOfMonth;
        public int Month { get; set; }
        public int Year { get; set; }
        public int? EveryXDays { get; set; }
        public int? Id;

        public string Description;
        
    }
}
