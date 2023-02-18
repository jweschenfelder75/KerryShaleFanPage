﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerryShaleFanPage.Shared.Enums;

namespace KerryShaleFanPage.Shared.Objects
{
    public class LogEntry : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime TimeStamp { get; set; }

        public LogLevelEnum LogLevel { get; set; }

        [StringLength(255)]
        public string? Logger { get; set; }

        [StringLength(255)]
        public string? Message { get; set; }

        [StringLength(1000)]
        public string? Exception { get; set; }
    }
}