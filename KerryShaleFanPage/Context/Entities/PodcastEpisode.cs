using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerryShaleFanPage.Shared.Objects
{
    public class PodcastEpisode : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public byte[]? ImageData { get; set; }

        [StringLength(5000)]
        public string? ImageDataBase64 { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(25)]
        public string? Duration { get; set; }  // e.g. 58 mins 38 secs

        [StringLength(40)]
        public string? Checksum { get; set; }  // e.g. A33398586A6D02628DFCDFD929219F45

        public DateTime? FetchedExpectedNextDate { get; set; }

        public DateTime? CalculatedExpectedNextDate { get; set; }
    }
}