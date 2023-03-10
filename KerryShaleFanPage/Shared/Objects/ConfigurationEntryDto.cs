using System;
using System.ComponentModel.DataAnnotations;

namespace KerryShaleFanPage.Shared.Objects
{
    [Serializable]
    public class ConfigurationEntryDto : BaseDto
    {
        [Key]
        public long Id { get; set; }

        [StringLength(255)]
        public string? Key { get; set; }

        [StringLength(1024)]
        public string? Value { get; set; }

        [StringLength(255)]
        public string? DataType { get; set; }
    }
}