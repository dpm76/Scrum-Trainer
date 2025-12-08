using System.Text.Json.Serialization;

namespace ScrumTrainer.Models;

public class Question
{
    [JsonPropertyName("question")]
    public string QuestionText { get; set; } = string.Empty;
    [JsonPropertyName("answers")]
    public string[] Answers { get; set; } = [];
    [JsonPropertyName("correct")]
    public int[] CorrectAnswerIndices { get; set; } = [];
}
