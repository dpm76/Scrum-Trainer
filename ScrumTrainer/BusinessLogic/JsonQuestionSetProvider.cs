using System.Text.Json;
using ScrumTrainer.Extensions;
using ScrumTrainer.Models;

namespace ScrumTrainer.BusinessLogic;

public class JsonQuestionSetProvider : IQuestionSetProvider
{
    private const string QUESTIONS_FILE_PATH = "Resources/questions.json";

    public ICollection<Question> ProvideQuestions(int questionsCount)
    {
        try
        {
            var jsonString = File.ReadAllText(QUESTIONS_FILE_PATH);
            var allQuestions = JsonSerializer.Deserialize<Question[]>(jsonString)
                ?? throw new ApplicationException("No questions found in JSON file");

            var random = new Random();
            return [.. allQuestions.TakeShuffled(questionsCount)];
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error loading questions from JSON file", ex);
        }
    }
}