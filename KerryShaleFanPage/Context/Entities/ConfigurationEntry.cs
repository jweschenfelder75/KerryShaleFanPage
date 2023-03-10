using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerryShaleFanPage.Context.Entities
{
    public class ConfigurationEntry : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [StringLength(255)]
        public string? Key { get; set; }

        [StringLength(1024)]
        public string? Value { get; set; }

        [StringLength(255)]
        public string? DataType { get; set; }

        public bool IsPassword { get; set; }

        [StringLength(255)]
        public string? Salt { get; set; }
    }
}