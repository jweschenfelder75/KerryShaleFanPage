using System;
using System.ComponentModel.DataAnnotations;
using KerryShaleFanPage.Shared.Enums;

namespace KerryShaleFanPage.Shared.Objects
{
    public class LogEntryDto : BaseDto
    {
        [Key]
        public long Id { get; set; }

        public DateTime TimeStamp { get; set; }

        public LogLevelEnum LogLevel { get; set; }

        [StringLength(255)]
        public string? Logger { get; set; }

        [StringLength(1024)]
        public string? Message { get; set; }

        [StringLength(1024)]
        public string? Exception { get; set; }
    }
}