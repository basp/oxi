namespace Oxi
{
    public interface IValue
    {
        ValueKind Kind { get; }

        IValue Clone();
    }

    public interface IValue<T> : IValue
    {
        T Value { get; }
    }
}