using System;
using System.ComponentModel.DataAnnotations;
using KerryShaleFanPage.Shared.Extensions;

namespace KerryShaleFanPage.Shared.Objects.ToDo.Twitter
{
    [Obsolete("TODO: Not finally implemented yet! Do not use!")]
    [Serializable]
    public class TwitterEpisode
    {
        [Key]
        public Guid Id { get; set; }

        public string? Title { get; set; }  // HTML encoded text

        public string? Description { get; set; }  // HTML encoded text

        public string? Date { get; set; }  // UTC Timestamp, e.g. 2023-01-26T12:04:10.000Z

        public string? ImageBaseUrl { get; set; }

        public string ImageType => "png";

        public string? ImageUrl => ImageBaseUrl;

        public string? Checksum => ComputeMd5Checksum();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string? ComputeMd5Checksum()
        {
            return $"{Title}|{Description}|{Date}|{ImageUrl}"
                .ComputeMd5();
        }
    }
}
