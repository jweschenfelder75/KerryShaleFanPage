using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerryShaleFanPage.Context.Entities
{
    public class PodcastEpisode
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(2500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ImageUrl { get; set; }

        public byte[]? ImageData { get; set; }

        [StringLength(5000)]
        public string? ImageDataBase64 { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(20)]
        public string? Duration { get; set; }  // e.g. 58 mins 38 secs

        public DateTime? FetchedExpectedNextDate { get; set; }

        public DateTime? CalculatedExpectedNextDate { get; set; }
    }
}