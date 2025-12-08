using System.Text;
using ScrumTrainer.BusinessLogic;

namespace ScrumTrainerTests;

public class JsonQuestionSetProviderTests: IDisposable
{
    private readonly string _resourcesDir =
        Path.Combine(Directory.GetCurrentDirectory(), "Resources");
    private readonly string _questionsFile;

    public JsonQuestionSetProviderTests()
    {
        _questionsFile = Path.Combine(_resourcesDir, "questions.json");
        Directory.CreateDirectory(_resourcesDir);
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_questionsFile)) File.Delete(_questionsFile);
            if (Directory.Exists(_resourcesDir)) Directory.Delete(_resourcesDir, recursive: true);
        }
        catch { /* ignore cleanup errors for tests */ }
    }


    private static string GenerateQuestionsJson(int count)
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (int i = 0; i < count; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append($@"{{""question"":""Question {i}"",""answers"":[""A"",""B"",""C""],""correct"":[0]}}");
        }
        sb.Append(']');
        return sb.ToString();
    }

    [Fact]
    public void LoadQuestions_LoadsExpectedNumberOfQuestions()
    {
        // Arrange
        int available = 5;
        int expected = 3;
        File.WriteAllText(_questionsFile, GenerateQuestionsJson(available));

        // Act
        var provider = new JsonQuestionSetProvider();
        var questions = provider.ProvideQuestions(expected);

        // Assert
        questions.Should().NotBeNull();
        questions.Count.Should().Be(expected);
        questions.All(q => !string.IsNullOrWhiteSpace(q.QuestionText)).Should().BeTrue();
        questions.All(q => q.Answers.Length >= 2).Should().BeTrue();
        questions.All(q => q.CorrectAnswerIndices.Length >= 1).Should().BeTrue();
        questions.All(q => q.Answers.All(a => !string.IsNullOrWhiteSpace(a))).Should().BeTrue();
    }
}