namespace Oxi
{
    using System;

    public abstract partial class Value
    {
        public class Integer : Value, IValue<int>, IFloatable, IOrdinal
        {
            public Integer(int value)
            {
                this.Value = value;
            }

            public override ValueKind Kind => ValueKind.Integer;

            public int Value { get; }

            public int OrdinalValue => (int)this.Value;

            public override bool IsTruthy => this.Value != 0;

            public Value.Float AsFloat() =>
                new Value.Float((long)this.Value);

            public override IValue Clone() => new Value.Integer(this.Value);

            public override string ToString() => this.Value.ToString();

            public override int GetHashCode() =>
                HashCode.Combine(this.Kind, this.Value.GetHashCode());

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                var other = obj as Value.Integer;
                if (other == null)
                {
                    return false;
                }

                return this.Value == other.Value;
            }

            public override IValue And(IValue value) =>
                value switch
                {
                    Value.Integer i => new Value.Integer(this.Value & i.Value),
                    _ => base.And(value),
                };

            public override IValue Or(IValue value) =>
                value switch
                {
                    Value.Integer i => new Value.Integer(this.Value | i.Value),
                    _ => base.And(value),
                };

            public override IValue Xor(IValue value) =>
                value switch
                {
                    Value.Integer i => new Value.Integer(this.Value ^ i.Value),
                    _ => base.And(value),
                };

            public IValue Add(IValue value) =>
                value switch
                {
                    Value.Integer x => new Value.Integer(this.Value + x.Value),
                    Value.Float x => new Value.Float(this.Value + x.Value),
                    _ => throw new NotSupportedException(),
                };

            public IValue Divide(IValue value) =>
                value switch
                {
                    Value.Integer y => new Value.Integer(this.Value / y.Value),
                    Value.Float y => new Value.Float(this.Value / y.Value),
                    _ => throw new NotSupportedException(),
                };

            public IValue Mul(IValue value) =>
                value switch
                {
                    Value.Integer y => new Value.Integer(this.Value * y.Value),
                    Value.Float y => new Value.Float(this.Value * y.Value),
                    _ => throw new NotSupportedException(),
                };

            public IValue Sub(IValue value) =>
                value switch
                {
                    Value.Integer y => new Value.Integer(this.Value - y.Value),
                    Value.Float y => new Value.Float(this.Value - y.Value),
                    _ => throw new NotSupportedException(),
                };

            public IValue Rem(IValue y) =>
                new Value.Integer(this.Value % ((Value.Integer)y).Value);

            public IValue Ord() => new Value.Integer(this.Value);

            public IValue Chr() => new Value.Character((char)this.Value);

            public IValue Pred() => new Value.Integer(this.Value - 1);

            public IValue Succ() => new Value.Integer(this.Value + 1);

            public IValue Acos() => this.AsFloat().Acos();

            public IValue Asin() => this.AsFloat().Asin();

            public IValue Atan() => this.AsFloat().Atan();

            public IValue Ceil() => this.AsFloat().Ceil();

            public IValue Cos() => this.AsFloat().Cos();

            public IValue Cosh() => this.AsFloat().Cosh();

            public IValue Exp() => this.AsFloat().Exp();

            public IValue Floor() => this.AsFloat().Floor();

            public IValue Log() => this.AsFloat().Log();

            public IValue Log10() => this.AsFloat().Log10();

            public IValue Sin() => this.AsFloat().Sin();

            public IValue Sinh() => this.AsFloat().Sinh();

            public IValue Sqrt() => this.AsFloat().Sqrt();

            public IValue Tan() => this.AsFloat().Tan();

            public IValue Tanh() => this.AsFloat().Tanh();

            public IValue Min(IValue value) =>
                value switch
                {
                    Value.Integer y =>
                        new Value.Integer(Math.Min(this.Value, y.Value)),
                    Value.Float y =>
                        new Value.Float(Math.Min(this.AsFloat().Value, y.Value)),
                    _ => throw new NotSupportedException(),
                };

            public IValue Max(IValue value) =>
                value switch
                {
                    Value.Integer y =>
                        new Value.Integer(Math.Max(this.Value, y.Value)),
                    Value.Float y =>
                        new Value.Float(Math.Max(this.AsFloat().Value, y.Value)),
                    _ => throw new NotSupportedException(),
                };

            public IValue Min(IOrdinal value) =>
                this.OrdinalValue < value.OrdinalValue
                    ? this.Clone()
                    : value.Clone();

            public IValue Max(IOrdinal value) =>
                this.OrdinalValue > value.OrdinalValue
                    ? this.Clone()
                    : value.Clone();

            public int CompareTo(IOrdinal value) =>
                this.OrdinalValue.CompareTo(value.OrdinalValue);

            public int CompareTo(IValue value) =>
                value switch
                {
                    Value.Integer y => this.Value.CompareTo(y.Value),
                    Value.Float y => this.Value.CompareTo(y.Value),
                    _ => throw new NotSupportedException(),
                };
        }
    }
}