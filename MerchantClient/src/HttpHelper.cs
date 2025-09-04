using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MerchantClient.src
{
    public class HttpHelper
    {
        private readonly int _maxRetries;
        private readonly int _delayMilliseconds;

        public HttpHelper(int maxRetries = 3, int delayMilliseconds = 1000)
        {
            _maxRetries = maxRetries;
            _delayMilliseconds = delayMilliseconds;
        }

        public async Task PostAsync(string url, object requestBody)
        {
            using var client = new HttpClient();

            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    var response = await client.PostAsync(url, content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"POST {url}");
                    Console.WriteLine($"Status Code: {response.StatusCode}");
                    Console.WriteLine("Response:");
                    Console.WriteLine(responseContent);

                    if (response.IsSuccessStatusCode)
                        break; // success, exit retry loop

                    // Optionally, you can decide which status codes to retry on
                    // For now, retry on any non-success status
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error calling {url} on attempt {attempt}: {ex.Message}");
                }

                if (attempt >= _maxRetries)
                {
                    Console.WriteLine($"Max retry attempts ({_maxRetries}) reached for {url}.");
                    break;
                }

                await Task.Delay(_delayMilliseconds);
                Console.WriteLine($"Retrying {url}, attempt {attempt + 1}...");
            }
        }
    }
}
