using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VirtualizedData.Services
{
    public class MovieService : IDataService
    {
        /// <summary>
        /// The page size for omdapi is 10
        /// </summary>
        public const int PageSize = 10;

        public async Task<MovieResponse> GetItemsAsync(string searchQuery, int page, CancellationToken cancellationToken = default (CancellationToken))
        {
            HttpClient client = new HttpClient();
            var requestUri = new Uri($"http://www.omdbapi.com/?s={searchQuery}&page={page}");
            var response = await client.GetAsync(requestUri, cancellationToken);
            var readAsStringAsync = await response.Content.ReadAsStringAsync();
            MovieResponse movieResponse = JsonConvert.DeserializeObject<MovieResponse>(readAsStringAsync);
            movieResponse.RemainingPages = (movieResponse.TotalResults/PageSize) - page;
            return movieResponse;
        }
    }
}