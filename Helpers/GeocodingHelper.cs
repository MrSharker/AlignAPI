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

        public GeocodingHelper()
        {
            _httpClient = new HttpClient();
        }
        public async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
        {
            double latitude = 0;
            double longitude = 0;
            try
            {
                var requestUri = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&addressdetails=1&limit=1";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("User-Agent", "YourApp/1.0");

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
