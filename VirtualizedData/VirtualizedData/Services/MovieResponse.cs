using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtualizedData.Services
{
    public class MovieResponse
    {
        [JsonProperty("Search")]
        public List<Movie> Movies { get; set; }
        public int TotalResults { get; set; }
        public bool Response { get; set; }
        public int RemainingPages { get; set; }
    }
}