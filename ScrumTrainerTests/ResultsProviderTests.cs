using ScrumTrainer.BusinessLogic;
using ScrumTrainer.Data;
using ScrumTrainer.Models;

namespace ScrumTrainerTests;

public class ResultsProviderTests
{
    [Fact]
    public async Task UserHasNoResultYet_GetQuizResults_ReturnsEmpty()
    {
        var resultsProvider = new ResultsProvider(new ApplicationUser(), new TestModelRepository<QuizResult>());

        var quizResults = await resultsProvider.QuizResults;

        quizResults.Should().BeEmpty();
    }

    [Fact]
    public async Task UserHasResults_GetQuizResults_ReturnsUserResults()
    {
        var user = new ApplicationUser();
        var results = new QuizResult []
        {
            new() { Id = 1, User = user },
            new() { Id = 2, User = user },
            new() { Id = 3, User = user }
        };

        var resultsProvider = new ResultsProvider(user, new TestModelRepository<QuizResult>(results, 4));

        var quizResults = await resultsProvider.QuizResults;

        quizResults.Should().Contain(results);
    }

    [Fact]
    public async Task UserHasNoResultsYetButOtherHas_GetQuizResults_ReturnsEmpty()
    {
        var user = new ApplicationUser();
        var results = new QuizResult []
        {
            new() { Id = 1, User = user },
            new() { Id = 2, User = user },
            new() { Id = 3, User = user }
        };

        var resultsProvider = new ResultsProvider(new ApplicationUser(), new TestModelRepository<QuizResult>(results, 4));

        var quizResults = await resultsProvider.QuizResults;

        quizResults.Should().BeEmpty();
    }

    [Fact]
    public async Task UserHasResultsAndOtherHas_GetQuizResults_ReturnsThoseOfUser()
    {
        var user1 = new ApplicationUser();
        var user2 = new ApplicationUser();
        var results = new QuizResult []
        {
            new() { Id = 1, User = user1 },
            new() { Id = 2, User = user1 },
            new() { Id = 3, User = user2 },
            new() { Id = 4, User = user1 },
            new() { Id = 5, User = user2 },
            new() { Id = 6, User = user1 }
        };

        var resultsProvider = new ResultsProvider(user1, new TestModelRepository<QuizResult>(results, 7));

        var quizResults = await resultsProvider.QuizResults;

        quizResults.Should().NotBeEmpty().And.OnlyContain(qr => qr.User == user1);
    }
}