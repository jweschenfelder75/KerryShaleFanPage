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
    [Obsolete("Obsolete: We will not use Twitter API anymore!")]
    public class TwitterCrawlApiService : ITwitterCrawlApiService
    {
        public string ConsumerKey => "";  // TODO: Make configurable!
        public string ConsumerSecret => "";  // TODO: Make configurable!

        private const string AuthorizationKey = "Authorization";
        private const string ContentTypeValue = "application/json";

        private readonly ILogger<TwitterCrawlApiService> _logger;  // TODO: Implement logging!

        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 
        /// </summary>
        public TwitterCrawlApiService(ILogger<TwitterCrawlApiService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc cref="ITwitterCrawlApiService"/>
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

            var result = await SendGetRequestAsync(baseUrl, queryParameters, cancellationToken);
            return JsonConvert.DeserializeObject<Users>(result ?? string.Empty);
        }

        /// <inheritdoc cref="ITwitterCrawlApiService"/>
        public async Task<Tweets?> GetTweetsAsync(string? userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
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

            var result = await SendGetRequestAsync(baseUrl, queryParameters, cancellationToken);
            return JsonConvert.DeserializeObject<Tweets>(result ?? string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="queryParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> SendGetRequestAsync(string baseUrl, NameValueCollection queryParameters, CancellationToken cancellationToken = default)
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
            var tweetResponse = await _httpClient.GetAsync(uriBuilder.ToString(), cancellationToken);

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
            _httpClient.DefaultRequestHeaders.Add(AuthorizationKey, $"Bearer {bearerToken}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypeValue));
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

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Add(AuthorizationKey, $"Basic {stringBearerRequest}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypeValue));
        }
    }
}
