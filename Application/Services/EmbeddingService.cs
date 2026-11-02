using Application.Interfaces.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace Application.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;

        public EmbeddingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> ExtractEmbeddingAsync(byte[] image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException(
                    "Image cannot be empty.", nameof(image));

            var request = new
            {
                image = Convert.ToBase64String(image)
            };

            using var response = await _httpClient.PostAsJsonAsync(
                "api/embedding",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Embedding API returned {(int)response.StatusCode}: {error}");
            }

            using var json = await response.Content.ReadFromJsonAsync<JsonDocument>();

            if (json == null)
                throw new InvalidOperationException(
                    "Embedding API returned an empty response.");

            var root = json.RootElement;

            var embeddings = root
                .GetProperty("results")[0]
                .GetProperty("embeddings");

            if (embeddings.ValueKind != JsonValueKind.Array ||
                embeddings.GetArrayLength() != 1)
            {
                throw new InvalidOperationException(
                    "Expected exactly one face in the image.");
            }

            string? embeddingBase64 = embeddings[0].GetString();

            if (string.IsNullOrEmpty(embeddingBase64))
                throw new InvalidOperationException(
                    "Embedding returned by API is empty.");

            return Convert.FromBase64String(embeddingBase64);
        }
    }
}
