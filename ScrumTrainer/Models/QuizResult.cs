using ScrumTrainer.Data;

namespace ScrumTrainer.Models;

public record QuizResult
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public int RightQuestionsCount { get; set; }
    public int TotalQuestionsCount { get; set; }
    public int SecondsTaken { get; set; }
    public int MaxSeconds { get; set; }
}