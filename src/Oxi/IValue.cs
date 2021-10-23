namespace Oxi
{
    public interface IValue
    {
        ValueKind Kind { get; }

        bool IsTruthy { get; }

        IValue Negate();

        IValue Clone();
    }

    public interface IValue<T> : IValue
    {
        T Value { get; }
    }
}