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
        // https://res.cloudinary.com/pippa/image/fetch/h_1400,w_1400,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b//63d0e60dba852b00110a0a75.jpg

        // Example2 ImageUrls:
        // https://res.cloudinary.com/pippa/image/fetch/h_500,w_500,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b/63d0e60dba852b00110a0a76.jpg 500w
        // https://res.cloudinary.com/pippa/image/fetch/h_750,w_750,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b/63d0e60dba852b00110a0a76.jpg 750w
        // https://res.cloudinary.com/pippa/image/fetch/h_1400,w_1400,f_auto/https://assets.pippa.io/shows/63d0e60777d9ee0011a4f45b/63d0e60dba852b00110a0a76.jpg 1400w  <= This is what we want!

        // Example: Href
        // /63d0e60777d9ee0011a4f45b/episodes/63d0e60dba852b00110a0a75

        // Example: ShowId
        // 63d0e60777d9ee0011a4f45b

        // Example: EpisodeId
        // 63d0e60dba852b00110a0a75

        private const string _EPISODES_SUBSTRING = "episodes";
        private const string _IMAGE_URL_TEMPLATE = "https://res.cloudinary.com/pippa/image/fetch/h_1400,w_1400,f_auto/https://assets.pippa.io/shows/{ShowId}{EpisodeId}.jpg";

        [Key]
        public Guid Id { get; set; }

        public string? Title => GetTitle();

        public string? Description { get; set; }  // HTML encoded text

        public string? Date { get; set; }  // Month (3 chars) and year (4 digits), e.g. Dec 2022

        public string? Duration { get; set; }  // Minutes and seconds, e.g. 58 min 38 sec

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

            var episodeId = ImageBaseUrl
                .Replace($"/{EpisodeShowId}", string.Empty)
                .Replace($"/{_EPISODES_SUBSTRING}", string.Empty);

            return _IMAGE_URL_TEMPLATE
                .Replace("{ShowId}", EpisodeShowId)
                .Replace("{EpisodeId}", episodeId);
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
            if (titleParts.Length == 12)  // 6 names, that should be extremely rare
            {
                return $"{titleParts[0]} {titleParts[1]} {titleParts[2]} {titleParts[3]} {titleParts[4]} {titleParts[5]}";
            }
            if (titleParts.Length == 10)  // 5 names, that should be very rare
            {
                return $"{titleParts[0]} {titleParts[1]} {titleParts[2]} {titleParts[3]} {titleParts[4]}";
            }
            if (titleParts.Length == 8)  // 4 names, e.g. John Michael of Doe
            {
                return $"{titleParts[0]} {titleParts[1]} {titleParts[2]} {titleParts[3]}";
            }
            if (titleParts.Length == 6)  // 3 names, e.g. John Michael Doe
            {
                return $"{titleParts[0]} {titleParts[1]} {titleParts[2]}";
            }
            if (titleParts.Length == 4)  // 2 names, e.g. John Doe
            {
                return $"{titleParts[0]} {titleParts[1]}";
            }
            return BaseTitle;
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