using System.Timers;
using ScrumTrainer.Data;
using ScrumTrainer.Models;
using Timer = System.Timers.Timer;

namespace ScrumTrainer.BusinessLogic;

public sealed class Quiz: IQuizStatusProvider, IDisposable
{
    public int CurrentQuestionIndex = 0;

    public ICollection<QuizQuestion> Questions { get; private set; } = default!;
    public int TimeLimitInSeconds { get; private set; }

    public int QuestionsCount{ get; private set; }

    public ObservableProperty<int> TimeTakenInSeconds { get; private set; } = new();

    public ObservableProperty<bool> IsCompleted { get; private set; } = new(false);

    public ObservableProperty<bool> IsStarted { get; private set; } = new(false);

    public bool IsStartDisabled { get => IsStarted || IsCompleted; }

    public bool IsNavigationDisabled { get => !IsStarted && !IsCompleted; }

    public bool IsFinishDisabled { get => !IsStarted; }

    public bool IsResetDisabled { get => !IsStarted && !IsCompleted; }

    public ApplicationUser User { get; set; }

    private readonly Timer _timer;

    private readonly IQuestionSetProvider _questionSetProvider;

    private bool _disposed = false;

    public int? QuestionsRightCount 
    { 
        get => IsCompleted 
                ? Questions?.Count( q => q.IsRight )
                : null;
    }

    public Quiz(int questionsCount, int timeLimitInSeconds): this(questionsCount, timeLimitInSeconds, new JsonQuestionSetProvider())
    {
    }

    public Quiz(int questionsCount, int timeLimitInSeconds, IQuestionSetProvider questionSetProvider)
    {
        TimeLimitInSeconds = timeLimitInSeconds;
        QuestionsCount = questionsCount;

        _questionSetProvider = questionSetProvider;

        _timer = new Timer(1000);
        _timer.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        TimeTakenInSeconds.Value++;

        if (TimeLimitInSeconds > 0 && TimeTakenInSeconds >= TimeLimitInSeconds)
        {
            FinishQuiz();
        }
    }

    public void StartQuiz()
    {
        if (IsStartDisabled)
            return;

        Questions = [.. _questionSetProvider.ProvideQuestions(QuestionsCount).Select(q => QuizQuestionBuilder.BuildQuestion(q, this))];
        CurrentQuestionIndex = 0;
        TimeTakenInSeconds.Value = 0;
        IsCompleted.Value = false;
        IsStarted.Value = true;
        _timer.Start();
    }

    public void GoToNextQuestion()
    {
        if (!IsNavigationDisabled && CurrentQuestionIndex < ((Questions?.Count ?? 0) - 1))
        {
            CurrentQuestionIndex++;
        }
    }

    public void GoToPreviousQuestion()
    {
        if (!IsNavigationDisabled && CurrentQuestionIndex > 0)
        {
            CurrentQuestionIndex--;
        }
    }

    public QuizQuestion? CurrentQuestion
    {
        get => CurrentQuestionIndex < (Questions?.Count ?? 0) 
                ? Questions?.ElementAt(CurrentQuestionIndex)
                : null;
    }

    public void FinishQuiz()
    {
        if (IsFinishDisabled)
            return;

        _timer.Stop();
        CurrentQuestionIndex = 0;
        IsCompleted.Value = true;
        IsStarted.Value = false;
    }

    public void ResetQuiz()
    {
        if (IsResetDisabled)
            return;

        _timer.Stop();
        CurrentQuestionIndex = 0;
        TimeTakenInSeconds.Value = 0;
        IsCompleted.Value = false;
        IsStarted.Value = false;
        Questions = default!;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    } 

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            _timer?.Dispose();

        _disposed = true;
        
    }

    ~Quiz()
    {
        Dispose(false);
    }
}
