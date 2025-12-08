namespace ScrumTrainer.Models;

public class QuizQuestion
{
    public required string Text { get; set; }

    public required ICollection<Answer> Answers { get; set; } 

    public bool IsMultiple { get; set; }

    public bool IsRight { get => Answers.All(a => a.IsCorrect == a.IsSelected); }

    private void ResetQuestion()
    {
        foreach (var answer in Answers)
        {
            answer.IsSelected = false;
        }
    }

    public void SelectSingleAnswer(int answerIndex)
    {
        ResetQuestion();
        Answers.ElementAt(answerIndex).IsSelected = true;
    }
}
