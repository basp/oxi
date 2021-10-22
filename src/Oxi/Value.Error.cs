namespace Oxi
{
    public abstract partial class Value
    {
        public class Error : Value, IValue
        {
            public static readonly IValue NONE = new Error("no error");

            public static readonly IValue TYPE = new Error("type mismatch");

            public static readonly IValue DIV = new Error("division by zero");

            public static readonly IValue PERM = new Error("permission denied");

            public static readonly IValue PROPNF = new Error("property not found");

            public static readonly IValue VERBNF = new Error("verb not found");

            public static readonly IValue VARNF = new Error("variable not found");

            public static readonly IValue INVIND = new Error("invalid indirection");

            public static readonly IValue RECMOVE = new Error("recursive move");

            public static readonly IValue MAXREC = new Error("too many verb calls");

            public static readonly IValue RANGE = new Error("range error");

            public static readonly IValue ARGS = new Error("incorrect number of arguments");

            public static readonly IValue NACC = new Error("move refused by destination");

            public static readonly IValue INVARG = new Error("invalid argument");

            public static readonly IValue QUOTA = new Error("resource limit exceeded");

            public static readonly IValue FLOAT = new Error("floating-point arithmetic error");

            internal Error(string value)
            {
                this.Message = value;
            }

            public override ValueKind Kind => ValueKind.Error;

            public string Message { get; }

            public override bool IsTruthy => false;

            public override IValue Clone() =>
                new Value.Error(this.Message);

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                var other = obj as Value.Error;
                if (other == null)
                {
                    return false;
                }

                return this.Message == other.Message;
            }

            public override int GetHashCode() =>
                this.Message.GetHashCode();

            public override string ToString() => this.Message;
        }
    }
}