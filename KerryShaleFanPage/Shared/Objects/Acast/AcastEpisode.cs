using System;
using System.ComponentModel.DataAnnotations;
using KerryShaleFanPage.Shared.Extensions;
using Newtonsoft.Json;

namespace KerryShaleFanPage.Shared.Objects.Acast
{
    [Serializable]
    public class AcastEpisode
    {
        // Example1 ImageUrls:
        // https://res.cloudinary.com/pippa/image/fetch/h_500,w_500,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b/63d0e60dba852b00110a0a75.jpg 500w
        // https://res.cloudinary.com/pippa/image/fetch/h_750,w_750,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b/63d0e60dba852b00110a0a75.jpg 750w
        // https://res.cloudinary.com/pippa/image/fetch/h_1400,w_1400,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b/63d0e60dba852b00110a0a75.jpg 1400w  <= This is what we want!

        // Example2 ImageUrls:
        // https://res.cloudinary.com/pippa/image/fetch/h_500,w_500,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b/1674641828496-d98fea216ec4f67b4eb8bbdd44f5d9dd.jpeg 500w
        // https://res.cloudinary.com/pippa/image/fetch/h_750,w_750,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b/1674641828496-d98fea216ec4f67b4eb8bbdd44f5d9dd.jpeg 750w
        // https://res.cloudinary.com/pippa/image/fetch/h_1400,w_1400,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b/1674641828496-d98fea216ec4f67b4eb8bbdd44f5d9dd.jpeg 1400w  <= This is what we want!

        private const string _SMALL_IMAGE_SUBSTRING = "h_500,w_500,";  // 500w
        private const string _MEDIUM_IMAGE_SUBSTRING = "h_750,w_750,";  // 750w
        private const string _LARGE_IMAGE_SUBSTRING = "h_1400,w_1400,";  // 1400w  <= This is what we want!

        [Key]
        public long Id { get; set; }

        public string? Title => GetTitle();

        public string? Description { get; set; }  // HTML encoded text

        public string? Date { get; set; }  // e.g. 1/22/2023

        public string? Duration { get; set; }  // Not available on Acast yet

        public string? ImageBaseUrl { get; set; }

        public string ImageType => "jpg";

        public string? ImageUrl => GetImageUrl();

        public string? Checksum => ComputeMd5Checksum();

        [JsonIgnore]
        public string? EpisodeShowId { get; set; }

        [JsonIgnore]
        public string? BaseTitle { get; set; }  // HTML encoded text

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string? GetImageUrl()
        {
            if (string.IsNullOrWhiteSpace(ImageBaseUrl))
            {
                return string.Empty;
            }

            return ImageBaseUrl
                .Replace(_SMALL_IMAGE_SUBSTRING, _LARGE_IMAGE_SUBSTRING)
                .Replace(_MEDIUM_IMAGE_SUBSTRING, _LARGE_IMAGE_SUBSTRING);
        }

        /// <summary>
        /// 
        /// We need to do this because the Acast fetch is providing a doubled title.
        /// </summary>
        /// <returns></returns>
        private string? GetTitle()
        {
            if (string.IsNullOrWhiteSpace(BaseTitle)) 
            { 
                return string.Empty; 
            }
            
            var titleParts = BaseTitle.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            return titleParts.Length switch
            {
                // 6 names, that should be extremely rare
                12 => $"{titleParts[0]} {titleParts[1]} {titleParts[2]} {titleParts[3]} {titleParts[4]} {titleParts[5]}",
                // 5 names, that should be very rare
                10 => $"{titleParts[0]} {titleParts[1]} {titleParts[2]} {titleParts[3]} {titleParts[4]}",
                // 4 names, e.g. John Michael of Doe
                8 => $"{titleParts[0]} {titleParts[1]} {titleParts[2]} {titleParts[3]}",
                // 3 names, e.g. John Michael Doe
                6 => $"{titleParts[0]} {titleParts[1]} {titleParts[2]}",
                // 2 names, e.g. John Doe
                4 => $"{titleParts[0]} {titleParts[1]}",
                // default
                _ => BaseTitle
            };
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