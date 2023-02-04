using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper.Configuration.Annotations;
using Newtonsoft.Json;

namespace KerryShaleFanPage.Shared.Objects
{
    [Serializable]
    public class PodcastEpisodeDto
    {
        private const int EpisodesTimePeriodInDays = 4 * 7;  // Every 4 weeks on a Sunday, TODO: Make configurable!

        private readonly DateTime _initialReferenceDate = new DateTime(2023, 01, 22, 0, 0, 0);  // It is a Sunday, TODO: Make configurable!

        [Key]
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
        public string? Duration { get; set; }  // e.g. "58 min 38 sec" or "00:58:38"

        public DateTime? FetchedExpectedNextDate { get; set; }

        [JsonIgnore, Ignore]
        public int CalculatedCurrentYear => DateTime.Now.Year;  // TODO: Make configurable! Never trust a computer or a human (but you can trust both if both are involved: 4 eyes). ;-)

        [JsonIgnore, Ignore]
        public DateTime CalculatedExpectedNextDate => CalculateExpectedNextDate();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DateTime CalculateExpectedNextDate()
        {
            var today = DateTime.Now.Date;
            var expectedNextDate = _initialReferenceDate;
            const int maxTries = 100;  // avoids an endless loop
            var count = 0;

            while (expectedNextDate < today && count < maxTries)
            {
                expectedNextDate = expectedNextDate.AddDays(EpisodesTimePeriodInDays);
                count++;
            }

            return expectedNextDate;
        }
    }
}