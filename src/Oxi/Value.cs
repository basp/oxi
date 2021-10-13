namespace Oxi
{
    public abstract partial class Value : IValue
    {
        public abstract ValueKind Kind { get; }

        public static bool IsZero(IValue value) =>
            value switch
            {
                Value.Boolean x => x.Value ? false : true,
                _ => false,
            };

        public static bool IsTruthy(IValue value) =>
            value switch
            {
                Value.Boolean x => x.Value,
                Value.Integer x => !IsZero(x),
                Value.Float x => !IsZero(x),
                _ => true,
            };

        public static bool IsType<T>(IValue value)
            where T : IValue => value is T;

        public abstract IValue Clone();

        public virtual IValue Not() =>
            new Value.Boolean(!IsTruthy(this));

        public virtual IValue And(IValue value) =>
            new Value.Boolean(IsTruthy(this) && IsTruthy(value));

        public virtual IValue Or(IValue value) =>
            new Value.Boolean(IsTruthy(this) || IsTruthy(value));

        public virtual IValue Xor(IValue value) =>
            new Value.Boolean(IsTruthy(this) ^ IsTruthy(value));
    }
}