namespace ScrumTrainer.BusinessLogic;

public interface IQuizStatusProvider
{
    public ObservableProperty<int> TimeTakenInSeconds { get; }
    public ObservableProperty<bool> IsCompleted { get; }
    public ObservableProperty<bool> IsStarted { get; }
}