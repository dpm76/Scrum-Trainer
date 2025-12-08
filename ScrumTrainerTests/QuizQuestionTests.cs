using ScrumTrainer.BusinessLogic;
using ScrumTrainer.Models;

namespace ScrumTrainerTests;

public class QuizQuestionTests
{
    [Fact]
    public void QuestionWithOneCorrectAnswer_IsMultiple_IsFalse()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2"
            ],
            CorrectAnswerIndices = [0]
        }, null);

        var isMultiple = question.IsMultiple;

        isMultiple.Should().BeFalse();
    }

    [Fact]
    public void QuestionWithSeveralCorrectAnswer_IsMultiple_IsTrue()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2",
                @"option3",
                @"option4"
            ],
            CorrectAnswerIndices = [0, 2]
        }, null);

        var isMultiple = question.IsMultiple;

        isMultiple.Should().BeTrue();
    }

    [Fact]
    public void QuestionWithNoSelectedAnswer_SelectSingleAnswer_OnlySuchAnswerIsSelected()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2",
                @"option3",
                @"option4"
            ],
            CorrectAnswerIndices = [0, 2]
        }, null);

        question.SelectSingleAnswer(1);

        question.Answers.Should()
            .ContainSingle( a => a.IsSelected && a.Index == 1)
            .And.NotContain( a => a.IsSelected && a.Index != 1);
    }

    [Fact]
    public void QuestionWithSelectedAnswer_SelectSingleAnswer_OnlySuchAnswerIsSelected()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2",
                @"option3",
                @"option4"
            ],
            CorrectAnswerIndices = [0, 2]
        }, null);
        question.SelectSingleAnswer(2);

        question.SelectSingleAnswer(1);

        question.Answers.Should()
            .ContainSingle( a => a.IsSelected && a.Index == 1)
            .And.NotContain( a => a.IsSelected && a.Index != 1);
    }

    [Fact]
    public void QuestionIsMultiple_WrongAnswersSelected_IsRightIsFalse()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2",
                @"option3",
                @"option4"
            ],
            CorrectAnswerIndices = [0, 2]
        }, null);

        question.Answers.ElementAt(1).IsSelected = true;
        question.Answers.ElementAt(3).IsSelected = true;

        question.IsRight.Should().BeFalse();
    }

    [Fact]
    public void QuestionIsMultiple_OneWrongAnswerSelected_IsRightIsFalse()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2",
                @"option3",
                @"option4"
            ],
            CorrectAnswerIndices = [0, 2]
        }, null);

        question.Answers.ElementAt(1).IsSelected = true;

        question.IsRight.Should().BeFalse();
    }

    [Fact]
    public void QuestionIsMultiple_OneRightAnswerSelected_IsRightIsFalse()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2",
                @"option3",
                @"option4"
            ],
            CorrectAnswerIndices = [0, 2]
        }, null);

        question.Answers.ElementAt(0).IsSelected = true;

        question.IsRight.Should().BeFalse();
    }

    [Fact]
    public void QuestionIsMultiple_AllRightAnswerSelected_IsRightIsTrue()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2",
                @"option3",
                @"option4"
            ],
            CorrectAnswerIndices = [0, 2]
        }, null);

        question.Answers.ElementAt(0).IsSelected = true;
        question.Answers.ElementAt(2).IsSelected = true;

        question.IsRight.Should().BeTrue();
    }

    [Fact]
    public void QuestionIsSingle_WrongAnswerSelected_IsRightIsFalse()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2",
                @"option3",
                @"option4"
            ],
            CorrectAnswerIndices = [1]
        }, null);

        question.SelectSingleAnswer(3);

        question.IsRight.Should().BeFalse();
    }

    [Fact]
    public void QuestionIsSingle_RightAnswerSelected_IsRightIsTrue()
    {
        var question = QuizQuestionBuilder.BuildQuestion(new Question
        {
            QuestionText = string.Empty,
            Answers =
            [
                @"option1",
                @"option2",
                @"option3",
                @"option4"
            ],
            CorrectAnswerIndices = [1]
        }, null);

        question.SelectSingleAnswer(1);

        question.IsRight.Should().BeTrue();
    }
}