using Microsoft.AspNetCore.Components;
using ScrumTrainer.BusinessLogic;
using ScrumTrainer.Models;

namespace ScrumTrainer.Components;

public partial class QuizViewer
{
    private const string AnswerStatusWrongCssClass = @"answer-wrong";
    private const string AnswerStatusMissedCssClass = @"answer-missed";
    private const string AnswerStatusRightCssClass = @"answer-right";
    private const string AnswerStatusNoneCssClass = @"answer-none";

    [Parameter] public required Quiz Quiz { get; set; }

    private static string GetCssClassFromAnswerStatus(AnswerStatus answerStatus)
        => answerStatus switch
        {
            AnswerStatus.Wrong => AnswerStatusWrongCssClass,
            AnswerStatus.Missed => AnswerStatusMissedCssClass,
            AnswerStatus.Right => AnswerStatusRightCssClass,
            _ => AnswerStatusNoneCssClass,
        };

    private string GetAnswerEnabledCssClass() 
        => Quiz.IsStarted ? @"answer-enabled" : @"answer-disabled";

    protected override async Task OnInitializedAsync()
    {
        Quiz.TimeTakenInSeconds.Changed += OnObservablePropertyChanged;
        Quiz.IsCompleted.Changed += OnObservablePropertyChanged;
        Quiz.IsStarted.Changed += OnObservablePropertyChanged;

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            Quiz.User = await UserManager.GetUserAsync(user) ?? default!;
        }
    }

    private void OnObservablePropertyChanged() 
        => InvokeAsync(StateHasChanged);

    private void Start() 
        => Quiz.StartQuiz();

    private void Previous() 
        => Quiz.GoToPreviousQuestion();

    private void Next() 
        => Quiz.GoToNextQuestion();

    private void Finish() 
        => Quiz.FinishQuiz();

    private void Reset() 
        =>  Quiz.ResetQuiz();
}