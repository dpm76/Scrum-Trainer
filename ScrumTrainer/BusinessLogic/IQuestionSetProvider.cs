using ScrumTrainer.Models;

namespace ScrumTrainer.BusinessLogic;

public interface IQuestionSetProvider
{
    ICollection<Question> ProvideQuestions(int questionsCount);
}