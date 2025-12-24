using ScrumTrainer.BusinessLogic;
using ScrumTrainer.Data;
using ScrumTrainer.Models;

namespace ScrumTrainerTests;

public class QuizTests
{
    private static IQuestionSetProvider CreateQuestionSetProvider()
    {
        var mockedProvider = new Mock<IQuestionSetProvider>();
        mockedProvider
            .Setup(p => p.ProvideQuestions(It.IsAny<int>()))
            .Returns((int n) =>
            {
                var questions = new List<Question>();
                for (int i=0; i<n; i++)
                {
                    questions.Add(
                        new Question
                        {
                            QuestionText = $"Question { i }",
                            Answers = [ @"answer 1", @"answer 2", @"answer 3" ],
                            CorrectAnswerIndices = [ 0 ]
                        });
                }

                return questions;
            });

        return mockedProvider.Object;
    }

    public class StartTest
    {
        [Fact]
        public async Task StartTest_CompletesAfterTimeLimit()
        {
            // Arrange
            using var quiz = new Quiz(questionsCount: 1, timeLimitInSeconds: 2, questionSetProvider: CreateQuestionSetProvider());

            // Act
            quiz.StartQuiz();

            // wait slightly more than the limit to ensure the timer has triggered
            await Task.Delay(3100);

            // Assert
            quiz.IsCompleted.Value.Should().BeTrue();
            quiz.TimeTakenInSeconds.Value.Should().BeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public void StartTest_QuizIsNotCompleted()
        {
            // Arrange
            using var quiz = new Quiz(questionsCount: 1, timeLimitInSeconds: 2, questionSetProvider: CreateQuestionSetProvider());

            // Act
            quiz.StartQuiz();

            // Assert
            quiz.IsCompleted.Value.Should().BeFalse();
        }

        [Fact]
        public async Task StartTest_TimerIsRunning()
        {
            // Arrange
            using var quiz = new Quiz(questionsCount: 1, timeLimitInSeconds: 20, questionSetProvider: CreateQuestionSetProvider());

            // Act
            quiz.StartQuiz();

            // wait slightly more than the limit to ensure the timer has triggered
            await Task.Delay(1100);

            // Assert
            quiz.TimeTakenInSeconds.Value.Should().BeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public void StartTest_CurrentIsFirstQuestion()
        {
            // Arrange
            using var quiz = new Quiz(questionsCount: 1, timeLimitInSeconds: 20, questionSetProvider: CreateQuestionSetProvider());

            // Act
            quiz.StartQuiz();

            // Assert
            quiz.CurrentQuestion.Should().Be(quiz.Questions.ElementAt(0));
        }
    }

    [Fact]
    public void GoToNextQuestion_CurrentIsSecondQuestion()
    {
        // Arrange
        using var quiz = new Quiz(questionsCount: 3, timeLimitInSeconds: 20, questionSetProvider: CreateQuestionSetProvider());

        // Act
        quiz.StartQuiz();
        quiz.GoToNextQuestion();

        // Assert
        quiz.CurrentQuestion.Should().Be(quiz.Questions.ElementAt(1));
    }

    [Fact]
    public void GoToPreviousQuestion_AfterGoToNextQuestion_CurrentIsFirstQuestion()
    {
        // Arrange
        using var quiz = new Quiz(questionsCount: 3, timeLimitInSeconds: 20, questionSetProvider: CreateQuestionSetProvider());

        // Act
        quiz.StartQuiz();
        quiz.GoToNextQuestion();        
        quiz.GoToPreviousQuestion();

        // Assert
        quiz.CurrentQuestion.Should().Be(quiz.Questions.ElementAt(0));
    }

    [Fact]
    public void GoToNextQuestion_OnLoopStopsAtLastQuestion()
    {
        // Arrange
        using var quiz = new Quiz(questionsCount: 3, timeLimitInSeconds: 20, questionSetProvider: CreateQuestionSetProvider());

        // Act
        quiz.StartQuiz();

        QuizQuestion? question;
        do{
            question = quiz.CurrentQuestion;
            quiz.GoToNextQuestion();
        }while(question != quiz.CurrentQuestion);

        // Assert
        quiz.CurrentQuestion.Should().Be(quiz.Questions.Last());
    }

    [Fact]
    public void GoToPreviousQuestion_OnLoopStopsAtFirstQuestion()
    {
        // Arrange
        using var quiz = new Quiz(questionsCount: 3, timeLimitInSeconds: 20, questionSetProvider: CreateQuestionSetProvider());

        // Act
        quiz.StartQuiz();
        QuizQuestion? question;
        do{
            question = quiz.CurrentQuestion;
            quiz.GoToPreviousQuestion();
        }while(question != quiz.CurrentQuestion);


        // Assert
        quiz.CurrentQuestion.Should().Be(quiz.Questions.First());
    }

    [Fact]
    public void ResetTest_QuizHasInitialValues()
    {
        // Arrange
        using var quiz = new Quiz(questionsCount: 2, timeLimitInSeconds: 20, questionSetProvider: CreateQuestionSetProvider());

        // Act
        quiz.StartQuiz();
        quiz.GoToNextQuestion();
        quiz.ResetQuiz();

        // Assert
        quiz.CurrentQuestion.Should().BeNull();
        quiz.IsCompleted.Value.Should().BeFalse();
        quiz.TimeTakenInSeconds.Value.Should().Be(0);
    }

    [Fact]
    public async Task FinishTest_QuizHasInitialValues()
    {
        // Arrange
        using var quiz = new Quiz(questionsCount: 2, timeLimitInSeconds: 20, questionSetProvider: CreateQuestionSetProvider());

        // Act
        quiz.StartQuiz();
        quiz.GoToNextQuestion();
        await Task.Delay(1100);
        quiz.FinishQuiz();

        // Assert
        quiz.IsCompleted.Value.Should().BeTrue();
        quiz.TimeTakenInSeconds.Value.Should().BeGreaterThan(0);
    }

    public class QuizNotStarted
    {
        [Fact]
        public void QuizNotStarted_InitialState_StartIsEnabled()
        {
            using var quiz = new Quiz(0, 10);
            quiz.IsStartDisabled.Should().BeFalse();
        }

        [Fact]
        public void QuizNotStarted_InitialState_NavigationIsDisabled()
        {
            using var quiz = new Quiz(0, 10);
            quiz.IsNavigationDisabled.Should().BeTrue();
        }

        [Fact]
        public void QuizNotStarted_InitialState_FinishIsDisabled()
        {
            using var quiz = new Quiz(0, 10);
            quiz.IsFinishDisabled.Should().BeTrue();
        }

        [Fact]
        public void QuizNotStarted_InitialState_ResetIsDisabled()
        {
            using var quiz = new Quiz(0, 10);
            quiz.IsResetDisabled.Should().BeTrue();
        }

        [Fact]
        public void QuizNotStarted_AfterStart_StartIsDisabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());

            quiz.StartQuiz();

            quiz.IsStartDisabled.Should().BeTrue();
        }

        [Fact]
        public void QuizNotStarted_AfterStart_NavigationIsEnabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());

            quiz.StartQuiz();

            quiz.IsNavigationDisabled.Should().BeFalse();
        }

        [Fact]
        public void QuizNotStarted_AfterStart_FinishIsEnabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());

            quiz.StartQuiz();

            quiz.IsFinishDisabled.Should().BeFalse();
        }

        [Fact]
        public void QuizNotStarted_AfterStart_ResetIsEnabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());

            quiz.StartQuiz();

            quiz.IsResetDisabled.Should().BeFalse();
        }
    }

    public class QuizStarted
    {
        [Fact]
        public void QuizStarted_AfterFinish_StartIsDisabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());
            quiz.StartQuiz();

            quiz.FinishQuiz();

            quiz.IsStartDisabled.Should().BeTrue();
        }

        [Fact]
        public void QuizStarted_AfterFinish_NavigationIsEnabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());
            quiz.StartQuiz();

            quiz.FinishQuiz();

            quiz.IsNavigationDisabled.Should().BeFalse();
        }

        [Fact]
        public void QuizStarted_AfterFinish_FinishIsDisabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());
            quiz.StartQuiz();

            quiz.FinishQuiz();

            quiz.IsFinishDisabled.Should().BeTrue();
        }

        [Fact]
        public void QuizStarted_AfterFinish_ResetIsEnabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());
            quiz.StartQuiz();

            quiz.FinishQuiz();

            quiz.IsResetDisabled.Should().BeFalse();
        }

        [Fact]
        public void QuizStarted_AfterReset_StartIsEnabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());
            quiz.StartQuiz();

            quiz.ResetQuiz();

            quiz.IsStartDisabled.Should().BeFalse();
        }

        [Fact]
        public void QuizStarted_AfterReset_NavigationIsDisabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());
            quiz.StartQuiz();

            quiz.ResetQuiz();

            quiz.IsNavigationDisabled.Should().BeTrue();
        }

        [Fact]
        public void QuizStarted_AfterReset_FinishIsDisabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());
            quiz.StartQuiz();

            quiz.ResetQuiz();

            quiz.IsFinishDisabled.Should().BeTrue();
        }

        [Fact]
        public void QuizStarted_AfterReset_ResetIsDisabled()
        {
            using var quiz = new Quiz(0, 10, questionSetProvider: CreateQuestionSetProvider());
            quiz.StartQuiz();

            quiz.ResetQuiz();

            quiz.IsResetDisabled.Should().BeTrue();
        }
    }

    [Fact]
    public void IsStartEnabled_StartQuiz_QuizIsStarted()
    {
        using var quiz = new Quiz(0, 10, CreateQuestionSetProvider());
        quiz.IsStartDisabled.Should().BeFalse(); //Precondition

        quiz.StartQuiz();

        quiz.IsStarted.Value.Should().BeTrue();
    }

    [Fact]
    public async Task IsStartDisabled_StartQuiz_QuizIsNotReStarted()
    {
        using var quiz = new Quiz(0, 10, CreateQuestionSetProvider());
        quiz.StartQuiz();
        quiz.IsStartDisabled.Should().BeTrue(); //Precondition

        await Task.Delay(2000);
        quiz.StartQuiz();

        quiz.IsStarted.Value.Should().BeTrue();
        quiz.TimeTakenInSeconds.Value.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void IsNavigationDisabled_GoToNextQuestion_QuestionRemains()
    {
        using var quiz = new Quiz(3, 10, CreateQuestionSetProvider());
        quiz.IsNavigationDisabled.Should().BeTrue(); //Precondition
        var question = quiz.CurrentQuestion;

        quiz.GoToNextQuestion();

        quiz.CurrentQuestion.Should().Be(question);
    }

    [Fact]
    public void IsNavigationEnabled_GoToNextQuestion_QuestionIsTheNextOne()
    {
        using var quiz = new Quiz(3, 10, CreateQuestionSetProvider());
        quiz.StartQuiz();
        quiz.IsNavigationDisabled.Should().BeFalse(); //Precondition

        quiz.GoToNextQuestion();

        quiz.CurrentQuestion.Should().Be(quiz.Questions.ElementAt(1));
    }

    [Fact]
    public void IsNavigationEnabled_GoToPreviousQuestion_QuestionIsThePreviousOne()
    {
        using var quiz = new Quiz(3, 10, CreateQuestionSetProvider());
        quiz.StartQuiz();
        quiz.GoToNextQuestion();
        quiz.IsNavigationDisabled.Should().BeFalse(); //Precondition

        quiz.GoToPreviousQuestion();

        quiz.CurrentQuestion.Should().Be(quiz.Questions.ElementAt(0));
    }

    [Fact]
    public void IsFinishDisabled_Finish_QuizIsNotCompleted()
    {
        using var quiz = new Quiz(3, 10, CreateQuestionSetProvider());
        quiz.IsFinishDisabled.Should().BeTrue(); //Precondition

        quiz.FinishQuiz();

        quiz.IsCompleted.Value.Should().BeFalse();
    }

    [Fact]
    public void IsFinishEnabled_Finish_QuizIsCompleted()
    {
        using var quiz = new Quiz(3, 10, CreateQuestionSetProvider());
        quiz.StartQuiz();
        quiz.IsFinishDisabled.Should().BeFalse(); //Precondition

        quiz.FinishQuiz();

        quiz.IsCompleted.Value.Should().BeTrue();
    }

    [Fact]
    public void IsResetEnabled_Reset_QuizIsNotStartedAndNotCompleted()
    {
        using var quiz = new Quiz(3, 10, CreateQuestionSetProvider());
        quiz.StartQuiz();
        quiz.IsResetDisabled.Should().BeFalse(); //Precondition

        quiz.ResetQuiz();

        quiz.IsStarted.Value.Should().BeFalse();
        quiz.IsCompleted.Value.Should().BeFalse();
    }

    [Fact]
    public void QuizStaredAndCurrentQuestionIsNotTheFirstOne_Finish_CurrentQuestionIsTheFirstOne ()
    {
        using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
        quiz.StartQuiz();
        quiz.GoToNextQuestion();
        quiz.CurrentQuestion.Should().NotBe(quiz.Questions.First()); //precondition

        quiz.FinishQuiz();

        quiz.CurrentQuestion.Should().Be(quiz.Questions.First());
    }

    [Fact]
    public void QuizIsCompletedAndHasThreeRightAnswers_QuestionsRightCount_ReturnsThree()
    {
        using var quiz = new Quiz(3, 10, CreateQuestionSetProvider());
        quiz.StartQuiz();

        QuizQuestion? question;
        do
        {
            quiz.CurrentQuestion?.SelectSingleAnswer(0);
            question = quiz.CurrentQuestion;
            quiz.GoToNextQuestion();
        } while(question != quiz.CurrentQuestion);
        quiz.FinishQuiz();
        var questionsRightCount = quiz.QuestionsRightCount;

        questionsRightCount.Should().Be(3);
    }

    public class Persistence
    {
        [Fact]
        public void QuizHasNotUser_AfterFinish_NoResultRecorded()
        {
            var testModelRepository = new TestModelRepository<QuizResult>();

            using var quiz = new Quiz(3, 10, CreateQuestionSetProvider(), testModelRepository);
            quiz.StartQuiz();
            quiz.FinishQuiz();

            testModelRepository.ModelSet.Should().BeEmpty();
        }

        [Fact]
        public void QuizHasUser_AfterFinish_NoResultRecorded()
        {
            var testModelRepository = new TestModelRepository<QuizResult>();

            using var quiz = new Quiz(3, 10, CreateQuestionSetProvider(), testModelRepository);
            quiz.User = new ApplicationUser();

            quiz.StartQuiz();
            quiz.FinishQuiz();

            testModelRepository.ModelSet.Should().NotBeEmpty();
        }
    }
}