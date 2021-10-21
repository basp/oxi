namespace Oxi
{
    public abstract partial class Value
    {
        public class Object : Value, IValue<int>
        {
            public Object(int value)
            {
                this.Value = value;
            }

            public int Value { get; }

            public override ValueKind Kind => ValueKind.Object;

            public override IValue Clone() =>
                new Value.Object(this.Value);

            public override string ToString() => $"#{this.Value}";
        }
    }
}