using System;
using System.ComponentModel.DataAnnotations;

namespace KerryShaleFanPage.Shared.Objects
{
    [Serializable]
    public class BaseDto
    {
        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }
    }
}
