using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace AlignAPI.Helpers
{
    public class GeocodingHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _userAgent;

        public GeocodingHelper(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _baseUrl = configuration["Nominatim:BaseUrl"];
            _userAgent = configuration["Nominatim:UserAgent"];
        }
        public async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
        {
            double latitude = 0;
            double longitude = 0;
            try
            {
                var requestUri = $"{_baseUrl}/search?q={Uri.EscapeDataString(address)}&format=json&addressdetails=1&limit=1";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("User-Agent", _userAgent);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var resultArray = JArray.Parse(jsonString);

                    if (resultArray.Count > 0)
                    {
                        var location = resultArray[0] as JObject;
                        if (location != null)
                        {
                            latitude = location.Value<double>("lat");
                            longitude = location.Value<double>("lon");
                        }
                    }
                }
                else 
                {
                    throw new Exception(response.ReasonPhrase);
                }
                return (latitude, longitude);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to geocode address: {address}: {ex.Message}");
            }
        }
    }
}
