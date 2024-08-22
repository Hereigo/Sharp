using System.ComponentModel.DataAnnotations;

namespace DotNet8.Models
{
    public class CalEventVM
    {
        public CalEventVM() { }

        [Required]
        public string Description;

        public CalEventVM(CalEvent calEvent)
        {
            Id = calEvent.Id;
            Description = calEvent.Description;
            Started = calEvent.Started;
            Time = calEvent.Time;
        }

        //public CalEventCategory Category { get; set; }

        [Required]
        public CalEventRepeat Repeat { get; set; }

        // public CalEventStatus Status { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Started { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "HH:mm")]
        public TimeSpan Time { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        // public IEnumerable<SelectListItem> RepeatList { get; set; }
        // public IEnumerable<SelectListItem> StatusList { get; set; }

        public int? EveryXDays { get; set; }
        public int? Id;
    }
}
