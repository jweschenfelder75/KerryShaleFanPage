using System;
using System.ComponentModel.DataAnnotations;
using KerryShaleFanPage.Shared.Extensions;

namespace KerryShaleFanPage.Shared.Objects.Acast
{
    [Serializable]
    public class ListenNotesEpisode
    {
        // Example1 ImageUrls:
        // https://production.listennotes.com/podcasts/is-it-rolling-bob/stewart-lee-bJAhoKEs6M1-jsFNXP6-fSY.300x300.jpg 300w
        // https://production.listennotes.com/podcasts/is-it-rolling-bob/stewart-lee-owWX4ivckZd-jsFNXP6-fSY.1400x1400.jpg 1400w  <= This is what we want!

        // Example2 ImageUrls:
        // https://production.listennotes.com/podcasts/is-it-rolling-bob/matt-rowland-hill-Q1_c293kxRI-E_xBUeLO8-y.300x300.jpg 300w
        // https://production.listennotes.com/podcasts/is-it-rolling-bob/matt-rowland-hill-S6WJ5jpnugU-E_xBUeLO8-y.1400x1400.jpg 1400w  <= This is what we want!

        [Key]
        public Guid Id { get; set; }

        public string? Title { get; set; }  // HTML encoded text

        public string? Description { get; set; }  // HTML encoded text

        public string? Date { get; set; }  // Month (3 chars), day (1-2 digits) and year (4 digits), e.g. Dec. 25, 2022

        public string? Duration { get; set; }  // Hours, minutes and seconds, e.g. 00:58:38

        public string? ImageBaseUrl { get; set; }

        public string ImageType => "jpg";

        public string? ImageUrl => GetImageUrl();

        public string? Checksum => ComputeMd5Checksum();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string? GetImageUrl()
        {
            return string.IsNullOrWhiteSpace(ImageBaseUrl) 
                ? string.Empty 
                : ImageBaseUrl;
        }

        /// <summary>
        /// 
        /// It is used to figure out if the data has changed without checking every single property. If the data changes, the checksum will do the same.
        /// </summary>
        /// <returns></returns>
        private string? ComputeMd5Checksum()
        {
            return $"{Title}|{Description}|{Date}|{Duration}|{ImageUrl}"
                    .ComputeMd5();
        }
    }
}