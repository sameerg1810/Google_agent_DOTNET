using System.ComponentModel.DataAnnotations;

namespace GenAiApp.Backend.Models;

public class ChatHistory
{
    [Key] // Defines 'Id' as the Primary Key in SQL
    public int Id { get; set; }

    [Required] // Ensures UserId is never null
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string UserPrompt { get; set; } = string.Empty;

    [Required]
    public string AiResponse { get; set; } = string.Empty;

    // Defaults to the exact moment the record is created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}