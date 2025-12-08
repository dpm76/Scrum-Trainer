using ScrumTrainer.BusinessLogic;

namespace ScrumTrainer.Models;

public enum AnswerStatus
{
    None,
    Wrong,
    Missed,
    Right
}

public class Answer
{
    public int Index { get; set; }
    public string Text { get; set; } = default!;
    public bool IsCorrect { get; set; }
    public bool IsSelected { get; set; }
    public IQuizStatusProvider? QuizStatusProvider { get; set; }

    public AnswerStatus AnswerStatus
    {
        get
        {
            if (!(QuizStatusProvider?.IsCompleted ?? true))
                return AnswerStatus.None;

            if(IsSelected && !IsCorrect)
                return AnswerStatus.Wrong;

            if(IsCorrect && !IsSelected)
                return AnswerStatus.Missed;

            if(IsCorrect && IsSelected)
                return AnswerStatus.Right;

            return AnswerStatus.None;
        }
    }
}