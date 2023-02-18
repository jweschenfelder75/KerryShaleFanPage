using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerryShaleFanPage.Shared.Objects
{
    public class Configuration : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(255)]
        public string? Key { get; set; }

        [StringLength(255)]
        public string? Value { get; set; }

        [StringLength(255)]
        public string? DataType { get; set; }
    }
}