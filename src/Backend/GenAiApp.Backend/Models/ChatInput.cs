namespace GenAiApp.Backend.Models;

/// <summary>
/// Data Transfer Object for receiving user chat requests.
/// </summary>
/// <param name="Prompt">The text message sent by the user.</param>
public record ChatInput(string Prompt);