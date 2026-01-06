using System.Security.Claims;
using GenAiApp.Backend.Services;
using GenAiApp.Backend.Models;

namespace GenAiApp.Backend.Extensions;

public static class EndpointExtensions
{
    public static void MapGlobalEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/ask", async (ChatInput input, GeminiService ai, ClaimsPrincipal user) =>
        {
            // 1. Call the AI service
            var reply = await ai.GenerateText(input.Prompt);

            // 2. Log metadata for debugging
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            Console.WriteLine($"User {userId} asked: {input.Prompt}");

            // 3. Log the AI Reply to the terminal
            Console.WriteLine($"Gemini Reply: {reply}");

            // 4. Return the result to Angular
            return Results.Ok(new { Reply = reply });
        });


        app.MapGet("/", () => "API is running!");
    }
}