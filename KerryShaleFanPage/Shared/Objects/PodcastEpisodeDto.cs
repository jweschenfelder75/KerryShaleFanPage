using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AutoMapper.Configuration.Annotations;
using Newtonsoft.Json;

namespace KerryShaleFanPage.Shared.Objects
{
    [Serializable]
    public class PodcastEpisodeDto
    {
        private const int _EPISODES_TIME_PERIOD_IN_DAYS = 4 * 7;  // Every 4 weeks on a Sunday, TODO: Make configurable!

        private readonly DateTime _initialReferenceDate = new DateTime(2023, 01, 22, 0, 0, 0);  // It is a Sunday, TODO: Make configurable!

        [Key]
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

        [JsonIgnore, Ignore]
        public int CalculatedCurrentYear => DateTime.Now.Year;  // TODO: Make configurable! Never trust a computer or a human (but you can trust both if both are involved: 4 eyes). ;-)

        [JsonIgnore, Ignore]
        public DateTime CalculatedExpectedNextDate => CalculateExpectedNextDate();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}"
                , "Date: ", Date?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), Environment.NewLine
                , "Title: ", Title, Environment.NewLine
                , "TweetText: ", $"New podcast episode is out: Kerry Shale and Lucas Hare interview \"{Title}\". The next episode will probably be published on {CalculatedExpectedNextDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}.", Environment.NewLine
                , "ImageUrl: ", ImageUrl, Environment.NewLine
                , "Description: ", Description, Environment.NewLine
                , "Duration: ", Duration, Environment.NewLine
                , "Checksum: ", Checksum, Environment.NewLine
                , "Id: ", Id, Environment.NewLine);
        }

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
                expectedNextDate = expectedNextDate.AddDays(_EPISODES_TIME_PERIOD_IN_DAYS);
                count++;
            }

            return expectedNextDate;
        }
    }
}