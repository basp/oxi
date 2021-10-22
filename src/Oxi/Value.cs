namespace Oxi
{
    public abstract partial class Value : IValue
    {
        public abstract ValueKind Kind { get; }

        public abstract bool IsTruthy { get; }

        public static bool IsZero(IValue value) =>
            value switch
            {
                Value.Boolean x => x.Value ? false : true,
                _ => false,
            };

        public static bool IsType<T>(IValue value)
            where T : IValue => value is T;

        public abstract IValue Clone();

        public virtual IValue Not() =>
            new Value.Boolean(!this.IsTruthy);

        public virtual IValue And(IValue other) =>
            new Value.Boolean(this.IsTruthy && other.IsTruthy);

        public virtual IValue Or(IValue other) =>
            new Value.Boolean(this.IsTruthy || other.IsTruthy);

        public virtual IValue Xor(IValue other) =>
            new Value.Boolean(this.IsTruthy ^ other.IsTruthy);
    }
}