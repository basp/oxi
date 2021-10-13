namespace Oxi
{
    public abstract partial class Value
    {
        public class Boolean : Value, IOrdinal
        {
            public Boolean(bool value)
            {
                this.Value = value;
            }

            public override ValueKind Kind => ValueKind.Boolean;

            public int OrdinalValue => this.Value ? 1 : 0;

            public bool Value { get; }

            public override IValue Clone() => new Boolean(this.Value);

            public override string ToString() =>
                this.Value.ToString().ToLower();

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                var other = obj as Value.Boolean;
                if (other == null)
                {
                    return false;
                }

                return this.Value == other.Value;
            }

            public override int GetHashCode() =>
                System.HashCode.Combine(this.Kind, this.Value.GetHashCode());

            public IValue Chr() => new Value.Character((char)this.OrdinalValue);

            public IValue Ord() => new Value.Integer(this.OrdinalValue);

            public IValue Succ() => this.Not();

            public IValue Pred() => this.Not();

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
        }
    }
}