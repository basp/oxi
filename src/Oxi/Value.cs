namespace Oxi
{
    using System;

    public abstract partial class Value : IValue
    {
        public abstract ValueKind Kind { get; }

        public abstract bool IsTruthy { get; }

        public static int _TypeOf(IValue value) =>
            value switch
            {
                Value.Boolean => (int)ValueKind.Boolean,
                Value.Integer => (int)ValueKind.Integer,
                Value.String => (int)ValueKind.String,
                Value.Float => (int)ValueKind.Float,
                Value.Object => (int)ValueKind.Object,
                _ => throw new NotSupportedException(value.Kind.ToString()),
            };

        public IValue TypeOf() => new Value.Integer(_TypeOf(this));

        public static bool IsZero(IValue value) =>
            value switch
            {
                Value.Boolean x => x.Value ? false : true,
                _ => false,
            };

        public static bool IsType<T>(IValue value)
            where T : IValue => value is T;

        public virtual IValue Negate() =>
            Value.Boolean.Get(!this.IsTruthy);

        public abstract IValue Clone();

        public virtual IValue Not() =>
            Value.Boolean.Get(!this.IsTruthy);

        public virtual IValue And(IValue other) =>
            Value.Boolean.Get(this.IsTruthy && other.IsTruthy);

        public virtual IValue Or(IValue other) =>
            Value.Boolean.Get(this.IsTruthy || other.IsTruthy);

        public virtual IValue Xor(IValue other) =>
            Value.Boolean.Get(this.IsTruthy ^ other.IsTruthy);
    }
}