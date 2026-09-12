using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartDesk.Api.Services;

public record TicketClassification
{
    [JsonPropertyName("urgency")]
    public required string Urgency { get; init; }

    [JsonPropertyName("category")]
    public required string Category { get; init; }

    [JsonPropertyName("suggested_reply")]
    public required string SuggestedReply { get; init; }
}

public interface ITicketClassifierService
{
    // Returns null on any failure (missing/invalid key, network error, timeout,
    // malformed response) — callers fall back to the default ticket values.
    Task<TicketClassification?> ClassifyAsync(string customer, string subject, CancellationToken cancellationToken);
}

// Uses Groq's free-tier, OpenAI-compatible chat completions API instead of a
// paid provider. HttpClient is injected via AddHttpClient in Program.cs, which
// also sets the base address and bearer token from GROQ_API_KEY.
public class TicketClassifierService(HttpClient httpClient, ILogger<TicketClassifierService> logger) : ITicketClassifierService
{
    private const string SystemPrompt = """
        You are a customer support classifier for a mobility aid company
        that makes electric walkers and wheelchairs.

        Read the customer ticket and respond ONLY with a valid JSON object.
        No explanation, no preamble, no markdown code blocks. Just raw JSON.

        The JSON must have exactly these fields:
        {
          "urgency": "LOW" or "MEDIUM" or "HIGH",
          "category": "hardware" or "software" or "billing" or "shipping" or "other",
          "suggested_reply": "a short, professional, empathetic reply to the customer (2-3 sentences)"
        }

        Urgency guide:
        - HIGH: safety risk, device unusable, customer cannot move safely
        - MEDIUM: device works but with problems, needs attention soon
        - LOW: billing, general questions, minor cosmetic issues
        """;

    public async Task<TicketClassification?> ClassifyAsync(string customer, string subject, CancellationToken cancellationToken)
    {
        try
        {
            var requestBody = new
            {
                model = "openai/gpt-oss-20b",
                max_tokens = 500,
                response_format = new { type = "json_object" },
                messages = new object[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = $"Customer: {customer}\nSubject: {subject}" },
                },
            };

            using var response = await httpClient.PostAsJsonAsync("chat/completions", requestBody, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogWarning(
                    "Groq classification failed with {StatusCode} for ticket from {Customer}: {Body}",
                    response.StatusCode, customer, errorBody);
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<GroqChatResponse>(cancellationToken: cancellationToken);
            var raw = payload?.Choices?.FirstOrDefault()?.Message?.Content;
            if (raw == null)
            {
                logger.LogWarning("Groq classification returned no content for ticket from {Customer}", customer);
                return null;
            }

            return JsonSerializer.Deserialize<TicketClassification>(raw);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Groq classification returned invalid JSON for ticket from {Customer}", customer);
            return null;
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Groq classification connection error for ticket from {Customer}", customer);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            logger.LogWarning(ex, "Groq classification timed out for ticket from {Customer}", customer);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Groq classification failed unexpectedly for ticket from {Customer}", customer);
            return null;
        }
    }

    // Minimal shape of a Groq/OpenAI chat completions response — only the fields we read.
    private record GroqChatResponse
    {
        [JsonPropertyName("choices")]
        public List<GroqChoice>? Choices { get; init; }
    }

    private record GroqChoice
    {
        [JsonPropertyName("message")]
        public GroqMessage? Message { get; init; }
    }

    private record GroqMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; init; }
    }
}
