namespace Oxi;

public interface IValue
{
    public interface IVisitor
    {
        void VisitBoolean(Value.Boolean node);

        void VisitInteger(Value.Integer node);

        void VisitCharacter(Value.Character node);

        void VisitString(Value.String node);

        void VisitFloat(Value.Float node);

        void VisitList(Value.List node);

        void VisitObject(Value.Object node);
    }

    ValueKind Kind { get; }

    bool IsTruthy { get; }

    IValue TypeOf();

    IValue Negate();

    IValue Clone();

    void Accept(IVisitor visitor);
}

public interface IValue<T> : IValue
{
    T Value { get; }
}
