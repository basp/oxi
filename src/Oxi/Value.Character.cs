namespace Oxi
{
    public abstract partial class Value
    {
        public class Character : Value, IOrdinal
        {
            public Character(char value)
            {
                this.Value = value;
            }

            public override ValueKind Kind => ValueKind.Character;

            public char Value { get; }

            public int OrdinalValue => (int)this.Value;

            public IValue Chr() => new Value.Character(this.Value);

            public override IValue Clone() => new Value.Character(this.Value);

            public override string ToString() => $"'{this.Value}";

            public override int GetHashCode() =>
                System.HashCode.Combine(this.Kind, this.Value.GetHashCode());

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                var other = obj as Value.Character;
                if (other == null)
                {
                    return false;
                }

                return this.Value == other.Value;
            }

            public IValue Max(IOrdinal value) =>
                this.OrdinalValue > value.OrdinalValue
                    ? this.Clone()
                    : value.Clone();

            public IValue Min(IOrdinal value) =>
                this.OrdinalValue < value.OrdinalValue
                    ? this.Clone()
                    : value.Clone();

            public IValue Ord() => new Value.Integer((int)this.Value);

            public IValue Pred() => new Value.Character((char)(this.Value - 1));

            public IValue Succ() => new Value.Character((char)(this.Value + 1));

            public int CompareTo(IOrdinal value) =>
                this.OrdinalValue.CompareTo(value.OrdinalValue);
        }
    }
}