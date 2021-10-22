namespace Oxi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract partial class Value
    {
        public class List : Value, IValue<IList<IValue>>
        {
            public List(IList<IValue> value)
            {
                this.Value = value;
            }

            public override ValueKind Kind => ValueKind.List;

            public override bool IsTruthy => this.Value.Count > 0;

            public IList<IValue> Value { get; }

            public override IValue Clone()
            {
                var xs = this.Value.Select(x => x.Clone()).ToList();
                return new Value.List(xs);
            }
        }
    }
}