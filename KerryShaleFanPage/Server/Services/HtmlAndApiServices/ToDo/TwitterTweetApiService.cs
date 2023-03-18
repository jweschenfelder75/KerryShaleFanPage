using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KerryShaleFanPage.Shared.Extensions;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices.ToDo;
using KerryShaleFanPage.Shared.Objects.ToDo.Twitter;

namespace KerryShaleFanPage.Server.Services.HtmlAndApiServices.ToDo
{
    [Obsolete("Obsolete: We will not use Twitter API anymore! Not finally implemented yet! Do not use!")]
    public class TwitterTweetApiService : ITwitterTweetApiService
    {
        public string ConsumerKey => "";  // TODO: Make configurable and encrypt!
        public string ConsumerSecret => "";  // TODO: Make configurable and encrypt!

        private const string _AUTHORIZATION_KEY = "Authorization";
        private const string _CONTENT_TYPE = "application/json";

        private readonly HttpClient _httpClient = new HttpClient();

        private readonly ILogger<TwitterTweetApiService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public TwitterTweetApiService(ILogger<TwitterTweetApiService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc cref="ITwitterTweetApiService"/>
        public async Task<Users?> GetUsersAsync(IList<string>? usernames, CancellationToken cancellationToken = default)
        {
            if (usernames == null || usernames.Count == 0)
            {
                return null;
            }

            const string baseUrl = "https://api.twitter.com/2/users/by";
            var queryParameters = new NameValueCollection
            {
                { "usernames", string.Join(',', usernames) },
                { "user.fields", "created_at,description,pinned_tweet_id" }
            };

            var result = await SendPostRequestAsync(baseUrl, queryParameters, cancellationToken);
            return JsonConvert.DeserializeObject<Users>(result ?? string.Empty);
        }

        /// TODO: UNFINISHED METHOD!
        /// <inheritdoc cref="ITwitterTweetApiService"/>
        public async Task<bool> SendTweetAsync(string? userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var baseUrl = $"https://api.twitter.com/2/users/{userId}/tweets";
            var queryParameters = new NameValueCollection
            {
                { "max_results", "15" },
                { "exclude", "retweets,replies" },
                { "expansions", "attachments.media_keys" },
                { "media.fields", "url" },
                { "tweet.fields", "created_at,text" }
            };

            await SendPostRequestAsync(baseUrl, queryParameters, cancellationToken);
            //return JsonConvert.DeserializeObject<Tweets>(result ?? string.Empty);
            return false;
        }

        /// <summary>
        /// TODO: UNFINISHED METHOD!
        /// 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="queryParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> SendPostRequestAsync(string baseUrl, NameValueCollection queryParameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return string.Empty;
            }

            var uriBuilder = new UriBuilder(baseUrl)
            {
                Query = queryParameters.ToQueryString()
            };

            await SetBearerTokenHeadersAsync(cancellationToken);
            var tweetResponse = await _httpClient.GetAsync(uriBuilder.ToString(), cancellationToken);  // Should be POST and not GET!

            if (tweetResponse.StatusCode == HttpStatusCode.OK)
            {
                return await tweetResponse.Content.ReadAsStringAsync(cancellationToken);
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="queryParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> SendAuthPostRequestAsync(string baseUrl, NameValueCollection queryParameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return string.Empty;
            }

            var uriBuilder = new UriBuilder(baseUrl)
            {
                Query = queryParameters.ToQueryString()
            };

            SetKeyAndTokenHeaders();
            var postResponse = await _httpClient.PostAsync(uriBuilder.ToString(), new StringContent(string.Empty), cancellationToken);

            if (postResponse.StatusCode == HttpStatusCode.OK)
            {
                return await postResponse.Content.ReadAsStringAsync(cancellationToken);
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ConsumerKey) || string.IsNullOrWhiteSpace(ConsumerSecret))
            {
                return string.Empty;
            }

            const string baseUrl = "https://api.twitter.com/oauth2/token";
            var queryParameters = new NameValueCollection
            {
                { "grant_type", "client_credentials" }
            };

            var result = await SendAuthPostRequestAsync(baseUrl, queryParameters, cancellationToken);
            var objectResponse = JObject.Parse(result);
            return objectResponse["access_token"]?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SetBearerTokenHeadersAsync(CancellationToken cancellationToken = default)
        {
            var bearerToken = await GetBearerTokenAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Add(_AUTHORIZATION_KEY, $"Bearer {bearerToken}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_CONTENT_TYPE));
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetKeyAndTokenHeaders()
        {
            if (string.IsNullOrWhiteSpace(ConsumerKey) || string.IsNullOrWhiteSpace(ConsumerSecret))
            {
                return;
            }

            //https://developer.twitter.com/en/docs/authentication/oauth-2-0/bearer-tokens

            var stringBearerRequest = $"{HttpUtility.UrlEncode(ConsumerKey)}:{HttpUtility.UrlEncode(ConsumerSecret)}";
            stringBearerRequest = Convert.ToBase64String(Encoding.UTF8.GetBytes(stringBearerRequest));

            // using var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Add(_AUTHORIZATION_KEY, $"Basic {stringBearerRequest}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_CONTENT_TYPE));
        }
    }
}
