namespace WpfUI.Common;

public sealed class Store<T>
{
    public Store(T value)
    {
        Value = value;
    }
    public T Value { get; set; }
}
