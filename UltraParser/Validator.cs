namespace UltraParser;

public class Validator<TProperty, T>
{
    public Validator(Func<TProperty, T, bool> condition, string errorMessage)
    {
        Condition = condition;
        ErrorMessage = errorMessage;
    }
    public Func<TProperty, T, bool> Condition { get; init; }
    public string ErrorMessage { get; init; }

    public string? Validate(T obj, TProperty property)
    {
        if (Condition(property, obj))
            return null;
        return ErrorMessage;
    }
}