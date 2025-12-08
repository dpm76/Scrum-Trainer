using ScrumTrainer.Models;

namespace ScrumTrainer.BusinessLogic;

public static class QuizQuestionBuilder
{
    public static QuizQuestion BuildQuestion(Question question, IQuizStatusProvider? quizStatusProvider)
    {
        var quizQuestion = new QuizQuestion{
            Text = question.QuestionText,
            Answers = new List<Answer>(question.Answers.Length),
            IsMultiple = question.CorrectAnswerIndices.Length > 1,
        };

        for (var i = 0; i < question.Answers.Length; i++)
        {
            var answer = question.Answers[i];
            quizQuestion.Answers.Add(new Answer 
            {
                Index = i,
                Text = answer,
                IsCorrect = question.CorrectAnswerIndices.Contains(i),
                QuizStatusProvider = quizStatusProvider
            });
        }

        return quizQuestion;
    }
}
