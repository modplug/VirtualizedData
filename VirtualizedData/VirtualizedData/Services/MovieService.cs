using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VirtualizedData.Services
{
    public class MovieService : IDataService
    {
        public const int PageSize = 10;
        public async Task<MovieResponse> GetItemsAsync(string searchQuery, int page)
        {
            HttpClient client = new HttpClient();
            var requestUri = new Uri($"http://www.omdbapi.com/?s={searchQuery}&page={page}");
            var response = await client.GetAsync(requestUri);
            var readAsStringAsync = await response.Content.ReadAsStringAsync();
            MovieResponse movieResponse = JsonConvert.DeserializeObject<MovieResponse>(readAsStringAsync);
            movieResponse.RemainingPages = (movieResponse.TotalResults/PageSize) - page;
//            Random rnd = new Random();
//            await Task.Delay(rnd.Next(500, 3000));
            return movieResponse;
        }
    }
}