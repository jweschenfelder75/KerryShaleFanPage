using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerryShaleFanPage.Context.Entities
{
    public class LogEntry : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime TimeStamp { get; set; }

        [StringLength(15)]
        public string? LogLevel { get; set; }

        [StringLength(255)]
        public string? Logger { get; set; }

        [StringLength(1024)]
        public string? Message { get; set; }

        [StringLength(1024)]
        public string? Exception { get; set; }
    }
}