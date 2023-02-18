using System;
using System.ComponentModel.DataAnnotations;
using KerryShaleFanPage.Shared.Attributes;

namespace KerryShaleFanPage.Shared.Objects
{
    [Serializable, EncryptedData]
    public class ConfigurationDto : BaseDto
    {
        [Key]
        public long Id { get; set; }

        [StringLength(255)]
        public string? Key { get; set; }

        [StringLength(255)]
        public string? Value { get; set; }

        [StringLength(255)]
        public string? OriginDataType { get; set; }
    }
}