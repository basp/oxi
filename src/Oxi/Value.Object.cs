namespace Oxi
{
    using System;

    public abstract partial class Value
    {
        public class Object : Value, IValue<int>, IOrdinal
        {
            public Object(int value)
            {
                this.Value = value;
            }

            public int Value { get; }

            public override ValueKind Kind => ValueKind.Object;

            public override bool IsTruthy => false;

            public int OrdinalValue => this.Value;

            public IValue Chr() =>
                throw new NotSupportedException();

            public int CompareTo(IOrdinal other) =>
                other switch
                {
                    Value.Object x => this.Value.CompareTo(x.Value),
                    _ => throw new NotSupportedException(),
                };

            public IValue Max(IOrdinal other) =>
                other switch
                {
                    Value.Object x => new Value.Object(
                        Math.Max(this.Value, x.Value)),
                    _ => throw new NotSupportedException(),
                };

            public IValue Min(IOrdinal other) =>
                other switch
                {
                    Value.Object x => new Value.Object(
                        Math.Min(this.Value, x.Value)),
                    _ => throw new NotSupportedException(),
                };

            public override IValue Clone() =>
                new Value.Object(this.Value);

            public IValue Ord() =>
                new Value.Integer(this.OrdinalValue);

            public IValue Pred() =>
                new Value.Object(this.Value - 1);

            public IValue Succ() =>
                new Value.Object(this.Value + 1);

            public override string ToString() => $"#{this.Value}";
        }
    }
}