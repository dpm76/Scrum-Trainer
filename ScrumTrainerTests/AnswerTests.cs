using ScrumTrainer.BusinessLogic;
using ScrumTrainer.Models;

namespace ScrumTrainerTests;

public class AnswerTests
{
    [Fact]
    public void AnswerIsCorrectAndIsSelected_AnswerStatus_IsRight()
    {
        var answer = new Answer
        {
            IsCorrect = true,
            IsSelected = true
        };

        var answerStatus = answer.AnswerStatus;

        answerStatus.Should().Be(AnswerStatus.Right);
    }

    [Fact]
    public void AnswerIsCorrectButNotSelected_AnswerStatus_IsMissed()
    {
        var answer = new Answer
        {
            IsCorrect = true,
            IsSelected = false
        };

        var answerStatus = answer.AnswerStatus;

        answerStatus.Should().Be(AnswerStatus.Missed);
    }

    [Fact]
    public void AnswerIsNotCorrectButSelected_AnswerStatus_IsWrong()
    {
        var answer = new Answer
        {
            IsCorrect = false,
            IsSelected = true
        };

        var answerStatus = answer.AnswerStatus;

        answerStatus.Should().Be(AnswerStatus.Wrong);
    }

    [Fact]
    public void AnswerIsNotCorrectNorSelected_AnswerStatus_IsNone()
    {
        var answer = new Answer
        {
            IsCorrect = false,
            IsSelected = false
        };

        var answerStatus = answer.AnswerStatus;

        answerStatus.Should().Be(AnswerStatus.None);
    }

    [Fact]
    public void QuizIsNotCompletedAndAnswerIsNotSelectedButIsCorrect_AnswerStatus_IsNone()
    {
        var quizStatusProviderMock = new Mock<IQuizStatusProvider>();
        quizStatusProviderMock.SetupGet( q => q.IsCompleted ).Returns( new ObservableProperty<bool>{ Value = false } ); 

        var answer = new Answer
        {
            IsCorrect = true,
            IsSelected = false,
            QuizStatusProvider = quizStatusProviderMock.Object
        };

        var answerStatus = answer.AnswerStatus;

        answerStatus.Should().Be(AnswerStatus.None);
    }

    [Fact]
    public void QuizIsCompletedAndAnswerIsNotSelectedButIsCorrect_AnswerStatus_IsMissed()
    {
        var quizStatusProviderMock = new Mock<IQuizStatusProvider>();
        quizStatusProviderMock.SetupGet( q => q.IsCompleted ).Returns( new ObservableProperty<bool>{ Value = true } ); 

        var answer = new Answer
        {
            IsCorrect = true,
            IsSelected = false,
            QuizStatusProvider = quizStatusProviderMock.Object
        };

        var answerStatus = answer.AnswerStatus;

        answerStatus.Should().Be(AnswerStatus.Missed);
    }
}