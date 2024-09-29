using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;


public class LlmClient
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    private readonly string? _systemPrompt;
    private readonly string? _model;
    private readonly float _temperature;
    private readonly int _maxTokens;


    public LlmClient(string? apiKey = null)
    {
        _httpClient = new HttpClient();
        _apiKey = apiKey;
        if (string.IsNullOrEmpty(_apiKey))
        {
            _apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("The ANTHROPIC_API_KEY environment variable is not set.");
            }
        }

        _systemPrompt = "You are a helpful assistant who is excellent at planning tasks and executing the plan in the correct order. If you don't know how to do something, answer \"I don't know how to do that.\"";

        _model = "claude-3-5-sonnet-20240620";
        _temperature = 0.1f;
        _maxTokens = 8192;
    }


    //    try
    //    {
    //        string prompt = "What is the capital of Peru?";
    //        Debug.WriteLine($"Asking: {prompt}");
    //        string response = await PerformQuery(prompt);
    //        //await File.WriteAllTextAsync("answer.txt", response);
    //        Debug.WriteLine($"Answer: {response}");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"An error occurred: {ex.Message}");
    //        if (ex.InnerException != null)
    //        {
    //            Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
    //        }
    //    }


    private async Task<string> CallAnthropicApi(JObject requestBody)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages")
        {
            Content = new StringContent(requestBody.ToString(), System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Add("x-api-key", _apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");

        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with status code {response.StatusCode}. Response content: {responseContent}");
        }

        return responseContent;
    }

    private async Task<string> RetryAnthropicApi(JObject requestBody, int maxAttempts = 10, int initialDelay = 1, int maxDelay = 60)
    {
        int attempt = 0;
        int delay = initialDelay;
        Random random = new Random();

        while (true)
        {
            attempt++;
            try
            {
                Debug.WriteLine($"Attempt #{attempt}");
                return await CallAnthropicApi(requestBody);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Attempt {attempt} failed: {e.Message}");

                if (attempt >= maxAttempts)
                {
                    throw new Exception($"Max attempts ({maxAttempts}) reached. Giving up.");
                }

                delay = Math.Min(maxDelay, delay * 2);
                double jitter = random.NextDouble() * 0.1 * delay;
                double sleepTime = delay + jitter;

                Debug.WriteLine($"Retrying in {sleepTime:F2} seconds...");
                await Task.Delay(TimeSpan.FromSeconds(sleepTime));
            }
        }
    }

    private static string? GetTextContent(JObject message)
    {
        var content = message["content"] as JArray;
        if (content == null) return null;

        foreach (var item in content)
        {
            if (item is JObject contentItem &&
                contentItem["type"]?.ToString() == "text" &&
                contentItem["text"] != null)
            {
                return contentItem["text"]?.ToString();
            }
        }
        return null;
    }

    public async Task<string> PerformQuery(string prompt)
    {
        var apiParams = new JObject
        {
            ["model"] = _model,
            ["max_tokens"] = _maxTokens,
            ["temperature"] = _temperature,
            ["system"] = _systemPrompt,
            ["messages"] = new JArray
            {
                new JObject
                {
                    ["role"] = "user",
                    ["content"] = new JArray
                    {
                        new JObject
                        {
                            ["type"] = "text",
                            ["text"] = prompt
                        }
                    }
                }
            }
        };

        Debug.WriteLine($"{apiParams}");
        var message = await RetryAnthropicApi(apiParams);
        var jsonMessage = JObject.Parse(message);
        Debug.WriteLine($"{jsonMessage}");
        return GetTextContent(jsonMessage) ?? throw new InvalidOperationException("Failed to get text content from API response.");
    }
}
