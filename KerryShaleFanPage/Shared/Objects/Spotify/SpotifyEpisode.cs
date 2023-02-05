using System;
using System.ComponentModel.DataAnnotations;
using KerryShaleFanPage.Shared.Extensions;

namespace KerryShaleFanPage.Shared.Objects.Spotify
{
    [Serializable]
    public class SpotifyEpisode
    {
        // Example1 ImageUrls:
        // https://lite-images-i.scdn.co/image/ab6765630000f68da6946432cdefc0d74208c2b7
        // https://i.scdn.co/image/ab6765630000f68da6946432cdefc0d74208c2b7 64w
        // https://i.scdn.co/image/ab67656300005f1fa6946432cdefc0d74208c2b7 300w
        // https://i.scdn.co/image/ab6765630000ba8aa6946432cdefc0d74208c2b7 640w  <= This is what we want!

        // Example2 ImageUrls:
        // https://lite-images-i.scdn.co/image/ab6765630000f68da6946432cdefc0d74208c2b7
        // https://i.scdn.co/image/ab6765630000f68dbe2a0e10f1a9ef6d28a93415 64w
        // https://i.scdn.co/image/ab67656300005f1fbe2a0e10f1a9ef6d28a93415 300w
        // https://i.scdn.co/image/ab6765630000ba8abe2a0e10f1a9ef6d28a93415 640w  <= This is what we want!

        private const string _SMALL_IMAGE_PREFIX = "ab6765630000f68d";  // 64w
        private const string _MEDIUM_IMAGE_PREFIX = "ab67656300005f1f";  // 300w
        private const string _LARGE_IMAGE_PREFIX = "ab6765630000ba8a";  // 640w  <= This is what we want!

        private const string _LITE_IMAGE_URL_PREFIX = "https://lite-images-i.scdn.co";
        private const string _DEFAULT_IMAGE_URL_PREFIX = "https://i.scdn.co";  // <= This is what we want!

        [Key]
        public Guid Id { get; set; }

        public string? Title { get; set; }  // HTML encoded text

        public string? Description { get; set; }  // HTML encoded text

        public string? Date { get; set; }  // Month (3 chars) and year (4 digits), e.g. Dec 2022

        public string? Duration { get; set; }  // Minutes and seconds, e.g. 58 min 38 sec

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
            if (string.IsNullOrWhiteSpace(ImageBaseUrl))
            {
                return string.Empty;
            }

            return ImageBaseUrl
                .Replace(_LITE_IMAGE_URL_PREFIX, _DEFAULT_IMAGE_URL_PREFIX)
                .Replace(_SMALL_IMAGE_PREFIX, _LARGE_IMAGE_PREFIX)
                .Replace(_MEDIUM_IMAGE_PREFIX, _LARGE_IMAGE_PREFIX);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string? ComputeMd5Checksum()
        {
            return $"{Title}|{Description}|{Date}|{Duration}|{ImageUrl}"
                    .ComputeMd5();
        }
    }
}