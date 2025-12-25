using ScrumTrainer.Data;
using ScrumTrainer.Models;

namespace ScrumTrainer.BusinessLogic;

public class ResultsProvider(ApplicationUser user, IModelRepository<QuizResult> modelRepository)
{
    private readonly ApplicationUser _user = user;
    private readonly IModelRepository<QuizResult> _modelRepository = modelRepository;

    public Task<ICollection<QuizResult>> QuizResults 
    {
        get => _modelRepository.FindOrDefault(qr => qr.User == _user); 
    }
}