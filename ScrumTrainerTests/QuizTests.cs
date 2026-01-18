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

    public class RightRate
    {
        [Fact]
        public void NotAllAnswersAreRight_RightRate_IsNotZero()
        {
            var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            foreach(var question in quiz.Questions)
            {
                question.SelectSingleAnswer(0);
            }
            quiz.Questions.ElementAt(0).SelectSingleAnswer(1);
            quiz.FinishQuiz();

            var rate = quiz.RightRate;

            rate.Should().BeGreaterThan(0d);
        }

        [Fact]
        public void AllAnswersAreRight_RightRate_IsOne()
        {
            var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            foreach(var question in quiz.Questions)
            {
                question.SelectSingleAnswer(0);
            }
            quiz.FinishQuiz();

            var rate = quiz.RightRate;

            rate.Should().Be(1d);
        }

        [Fact]
        public void AllAnswersAreWrong_RightRate_IsZero()
        {
            var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            foreach(var question in quiz.Questions)
            {
                question.SelectSingleAnswer(1);
            }
            quiz.FinishQuiz();

            var rate = quiz.RightRate;

            rate.Should().Be(0d);
        }
    }

    public class GoToNextFailedQuestion
    {
        [Fact]
        public void StartWithMultipleFailedQuestions_GoToNextFailedQuestion_NavigatesToNextFailedQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();

            // Mark some questions as failed (by selecting wrong answer)
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 0 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 1 - Correct
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 2 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 3 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 4 - Correct

            // Go back to the first question
            quiz.CurrentQuestionIndex = 0;

            // Act
            quiz.GoToNextFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(2); // Next failed question after 0
        }

        [Fact]
        public void FromMiddleOfQuiz_GoToNextFailedQuestion_NavigatesToNextFailedQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();

            // Mark some questions as failed
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 0 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 1 - Correct
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 2 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 3 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 4 - Correct

            // Start from question 1
            quiz.CurrentQuestionIndex = 1;

            // Act
            quiz.GoToNextFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(2); // Next failed question
        }

        [Fact]
        public void NoFailedQuestionsAfterCurrent_GoToNextFailedQuestion_RemainsAtCurrentQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();

            // Mark some questions as failed
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 0 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 1 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 2 - Correct
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 3 - Correct
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 4 - Correct

            // Start from question 3 (no failed questions after this)
            quiz.CurrentQuestionIndex = 3;

            // Act
            quiz.GoToNextFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(3); // Remains at current question
        }

        [Fact]
        public void AllFailedQuestions_GoToNextFailedQuestion_NavigatesToFirstFailedAfterCurrent()
        {
            // Arrange
            using var quiz = new Quiz(3, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();

            // Mark all questions as failed
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 0 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 1 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 2 - Failed

            // Start from question 0
            quiz.CurrentQuestionIndex = 0;

            // Act
            quiz.GoToNextFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(1); // Next failed question
        }

        [Fact]
        public void QuizNotStarted_GoToNextFailedQuestion_DoesNotNavigate()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());

            // Act
            quiz.GoToNextFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(0);
        }
    }

    public class GoToPreviousFailedQuestion
    {
        [Fact]
        public void StartWithMultipleFailedQuestions_GoToPreviousFailedQuestion_NavigatesToLastFailedQuestionBefore()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();

            // Mark some questions as failed
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 0 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 1 - Correct
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 2 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 3 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 4 - Correct

            // Start from question 4
            quiz.CurrentQuestionIndex = 4;

            // Act
            quiz.GoToPreviousFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(3); // Previous failed question
        }

        [Fact]
        public void FromMiddleOfQuiz_GoToPreviousFailedQuestion_NavigatesToPreviousFailedQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();

            // Mark some questions as failed
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 0 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 1 - Correct
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 2 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 3 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 4 - Correct

            // Start from question 3
            quiz.CurrentQuestionIndex = 3;

            // Act
            quiz.GoToPreviousFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(2); // Previous failed question
        }

        [Fact]
        public void NoFailedQuestionsBeforeCurrent_GoToPreviousFailedQuestion_RemainsAtCurrentQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();

            // Mark some questions as failed
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 0 - Correct
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 1 - Correct
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 2 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 3 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 4 - Failed

            // Start from question 1 (no failed questions before this)
            quiz.CurrentQuestionIndex = 1;

            // Act
            quiz.GoToPreviousFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(1); // Remains at current question
        }

        [Fact]
        public void AllFailedQuestions_GoToPreviousFailedQuestion_NavigatesToLastFailedBeforeCurrent()
        {
            // Arrange
            using var quiz = new Quiz(3, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();

            // Mark all questions as failed
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 0 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 1 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 2 - Failed

            // Start from question 2
            quiz.CurrentQuestionIndex = 2;

            // Act
            quiz.GoToPreviousFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(1); // Previous failed question
        }

        [Fact]
        public void QuizNotStarted_GoToPreviousFailedQuestion_DoesNotNavigate()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());

            // Act
            quiz.GoToPreviousFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(0);
        }

        [Fact]
        public void OnFirstQuestion_GoToPreviousFailedQuestion_RemainsOnFirstQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();

            // Mark some questions as failed
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 0 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(1); // Question 1 - Failed
            quiz.GoToNextQuestion();
            quiz.CurrentQuestion?.SelectSingleAnswer(0); // Question 2 - Correct

            // Start from question 0
            quiz.CurrentQuestionIndex = 0;

            // Act
            quiz.GoToPreviousFailedQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(0); // Remains on first question
        }
    }

    public class GoToFirstQuestion
    {
        [Fact]
        public void QuizStarted_GoToFirstQuestion_CurrentIsFirstQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            quiz.GoToNextQuestion();
            quiz.GoToNextQuestion();
            quiz.GoToNextQuestion();
            quiz.CurrentQuestionIndex.Should().Be(3); // Precondition

            // Act
            quiz.GoToFirstQuestion();

            // Assert
            quiz.CurrentQuestion.Should().Be(quiz.Questions.ElementAt(0));
            quiz.CurrentQuestionIndex.Should().Be(0);
        }

        [Fact]
        public void QuizStarted_FromLastQuestion_GoToFirstQuestion_CurrentIsFirstQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            quiz.CurrentQuestionIndex = quiz.Questions.Count - 1;
            quiz.CurrentQuestionIndex.Should().Be(4); // Precondition

            // Act
            quiz.GoToFirstQuestion();

            // Assert
            quiz.CurrentQuestion.Should().Be(quiz.Questions.First());
            quiz.CurrentQuestionIndex.Should().Be(0);
        }

        [Fact]
        public void QuizNotStarted_GoToFirstQuestion_DoesNotNavigate()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.IsNavigationDisabled.Should().BeTrue(); // Precondition

            // Act
            quiz.GoToFirstQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(0);
        }

        [Fact]
        public void AlreadyOnFirstQuestion_GoToFirstQuestion_RemainsOnFirstQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            quiz.CurrentQuestionIndex.Should().Be(0); // Precondition

            // Act
            quiz.GoToFirstQuestion();

            // Assert
            quiz.CurrentQuestion.Should().Be(quiz.Questions.First());
            quiz.CurrentQuestionIndex.Should().Be(0);
        }
    }

    public class GoToLastQuestion
    {
        [Fact]
        public void QuizStarted_GoToLastQuestion_CurrentIsLastQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            quiz.CurrentQuestionIndex.Should().Be(0); // Precondition

            // Act
            quiz.GoToLastQuestion();

            // Assert
            quiz.CurrentQuestion.Should().Be(quiz.Questions.ElementAt(4));
            quiz.CurrentQuestionIndex.Should().Be(4);
        }

        [Fact]
        public void QuizStarted_FromMiddle_GoToLastQuestion_CurrentIsLastQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            quiz.GoToNextQuestion();
            quiz.GoToNextQuestion();
            quiz.CurrentQuestionIndex.Should().Be(2); // Precondition

            // Act
            quiz.GoToLastQuestion();

            // Assert
            quiz.CurrentQuestion.Should().Be(quiz.Questions.Last());
            quiz.CurrentQuestionIndex.Should().Be(4);
        }

        [Fact]
        public void QuizNotStarted_GoToLastQuestion_DoesNotNavigate()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.IsNavigationDisabled.Should().BeTrue(); // Precondition

            // Act
            quiz.GoToLastQuestion();

            // Assert
            quiz.CurrentQuestionIndex.Should().Be(0);
        }

        [Fact]
        public void AlreadyOnLastQuestion_GoToLastQuestion_RemainsOnLastQuestion()
        {
            // Arrange
            using var quiz = new Quiz(5, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            quiz.CurrentQuestionIndex = quiz.Questions.Count - 1;
            quiz.CurrentQuestionIndex.Should().Be(4); // Precondition

            // Act
            quiz.GoToLastQuestion();

            // Assert
            quiz.CurrentQuestion.Should().Be(quiz.Questions.Last());
            quiz.CurrentQuestionIndex.Should().Be(4);
        }

        [Fact]
        public void SingleQuestion_GoToLastQuestion_CurrentIsTheOnlyQuestion()
        {
            // Arrange
            using var quiz = new Quiz(1, 10, CreateQuestionSetProvider());
            quiz.StartQuiz();
            quiz.CurrentQuestionIndex.Should().Be(0); // Precondition

            // Act
            quiz.GoToLastQuestion();

            // Assert
            quiz.CurrentQuestion.Should().Be(quiz.Questions.First());
            quiz.CurrentQuestionIndex.Should().Be(0);
        }
    }
}