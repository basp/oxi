namespace Oxi;

public abstract partial class Value
{
    public class Return : Value, IValue<IValue>
    {
        public Return(IValue value)
        {
            this.Value = value;
        }

        public IValue Value { get; }

        public override ValueKind Kind =>
            ValueKind.Return;

        public override bool IsTruthy =>
            this.Value.IsTruthy;

        public override IValue Clone() =>
            new Value.Return(this.Value);

        public override void Accept(IValue.IVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}
