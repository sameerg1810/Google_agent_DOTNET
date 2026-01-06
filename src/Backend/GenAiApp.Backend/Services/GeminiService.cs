using Google.GenAI;

namespace GenAiApp.Backend.Services;

public class GeminiService(IConfiguration config)
{
    private readonly string _projectId = config["GoogleCloud:ProjectId"] ?? "project-8f2d09ba-e77c-477d-a34";

    public async Task<string> GenerateText(string prompt)
    {
        var client = new Client(project: _projectId, location: "us-central1", vertexAI: true);
        var response = await client.Models.GenerateContentAsync(
            model: "gemini-2.5-flash",
            contents: prompt
        );
        return response.Candidates[0].Content.Parts[0].Text;
    }
}