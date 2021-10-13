namespace Oxi
{
    public interface IValue
    {
        ValueKind Kind { get; }

        IValue Clone();
    }
}