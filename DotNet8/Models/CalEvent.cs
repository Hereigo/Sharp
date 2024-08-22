﻿using System.ComponentModel.DataAnnotations;

namespace DotNet8.Models
{
    public enum CalEventStatus
    {
        Active,
        Disabled,
        Deleted
    }

    public enum CalEventRepeat
    {
        Once,
        Yearly,
        Monthly,
        EveryXdays,
    }

    public class CalEvent
    {
        public int Id { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int? EveryXDays { get; set; }

        public CalEventCategory? Category { get; set; }
        public CalEventRepeat Repeat { get; set; }
        public CalEventStatus Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime Modified { get; set; }

        [DataType(DataType.Date)]
        public DateTime Started { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "HH:mm")]
        public TimeSpan Time { get; set; }

        public string Description { get; set; }

        public CalEvent() { }

        public CalEvent(DateTime date, string text = "")
        {
            Day = date.Day;
            Description = text;
            EveryXDays = null;
            Modified = date;
            Month = date.Month;
            Repeat = CalEventRepeat.Once;
            Started = date;
            Status = CalEventStatus.Active;
            Year = date.Year;
        }
    }

    public class CalEventCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public CalEventCategory()
        {
            Name = "default";
        }

        public CalEventCategory(string categoryName)
        {
            Name = categoryName;
        }
    }
}
