namespace ScrumTrainer;

public sealed class ScrumTrainerConfiguration
{
    public int QuestionsCount { get; set; }
    public int TimeLimitInSeconds { get; set; }
    public required string LinkedInLink { get; set; }
    public required string PortfolioLink { get; set; }
    public required string SourceCodeLink { get; set; }
    public required string ContactEmail { get; set; }
}
