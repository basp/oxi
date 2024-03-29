namespace Oxi;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract partial class Value
{
    public class List : Value, IValue<IList<IValue>>, IAggregate
    {
        public static readonly IValue Empty = new List();

        public List()
        {
            this.Value = new List<IValue>();
        }

        public List(IList<IValue> value)
        {
            this.Value = value;
        }

        public override ValueKind Kind => ValueKind.List;

        public override bool IsTruthy => this.Value.Count > 0;

        public IList<IValue> Value { get; }

        public int Size => throw new NotImplementedException();

        public IList<IValue> Elements => this.Value;

        public IValue At(int i)
        {
            throw new NotImplementedException();
        }

        public override IValue Clone()
        {
            var xs = this.Value.Select(x => x.Clone()).ToList();
            return new Value.List(xs);
        }

        public override void Accept(IValue.IVisitor visitor) =>
            visitor.VisitList(this);

        public override string ToString()
        {
            var xs = this.Value.Select(x => x.ToString());
            return $"{{{string.Join(", ", xs)}}}";
        }

        public IValue Concat(IAggregate value)
        {
            throw new NotImplementedException();
        }

        public IValue Cons(IValue value)
        {
            throw new NotImplementedException();
        }

        public IAggregate Drop(int n)
        {
            throw new NotImplementedException();
        }

        public IValue First() =>
            this.Elements.FirstOrDefault();

        public IEnumerator<IValue> GetEnumerator() =>
            this.Elements.GetEnumerator();

        public IAggregate Rest()
        {
            var xs = this.Elements.Skip(1).ToList();
            return new Value.List(xs);
        }

        public IAggregate Take(int n)
        {
            var xs = this.Elements.Take(n).ToList();
            return new Value.List(xs);
        }

        public IValue Uncons(out IAggregate rest)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
