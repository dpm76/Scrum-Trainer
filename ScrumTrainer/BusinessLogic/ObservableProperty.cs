namespace ScrumTrainer.BusinessLogic;

public class ObservableProperty<T>
{
    public ObservableProperty()
    {
        Value = default;
    }

    public ObservableProperty(T value)
    {
        Value = value;
    }

    private T? _value;

    public T? Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                Changed?.Invoke();
            }
        }
    }

    public event Action? Changed;

    public static implicit operator T?(ObservableProperty<T> observableProperty)
    {
        return observableProperty.Value;
    }

    public static implicit operator string(ObservableProperty<T> observableProperty)
    {
        return observableProperty.Value?.ToString() ?? string.Empty;
    }
}
