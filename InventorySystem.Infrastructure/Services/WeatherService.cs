using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventorySystem.Infrastructure.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _apiKey = "e03eb7332381416cb6414943252505"; 


        public async Task<string> GetCurrentWeatherAsync(string city)
        {
            var url = $"https://api.weatherapi.com/v1/current.json?key={_apiKey}&q={city}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return $"Error: {response.ReasonPhrase}";

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var location = doc.RootElement.GetProperty("location").GetProperty("name").GetString();
            var country = doc.RootElement.GetProperty("location").GetProperty("country").GetString();
            var tempC = doc.RootElement.GetProperty("current").GetProperty("temp_c").GetDecimal();
            var condition = doc.RootElement.GetProperty("current").GetProperty("condition").GetProperty("text").GetString();

            return $"Weather in {location}, {country}: {condition}, {tempC}°C";
        }
    }
}
