namespace Oxi
{
    public abstract partial class Value
    {
        public class Error : Value, IValue
        {
            public static readonly IValue NONE = new Error("No error");

            public static readonly IValue TYPE = new Error("Type mismatch");

            public static readonly IValue DIV = new Error("Division by zero");

            public static readonly IValue PERM = new Error("Permission denied");

            public static readonly IValue PROPNF = new Error("Property not found");

            public static readonly IValue VERBNF = new Error("Verb not found");

            public static readonly IValue VARNF = new Error("Variable not found");

            public static readonly IValue INVIND = new Error("Invalid indirection");

            public static readonly IValue RECMOVE = new Error("Recursive move");

            public static readonly IValue MAXREC = new Error("Too many verb calls");

            public static readonly IValue RANGE = new Error("Range error");

            public static readonly IValue ARGS = new Error("Incorrect number of arguments");

            public static readonly IValue NACC = new Error("Move refused by destination");

            public static readonly IValue INVARG = new Error("Invalid argument");

            public static readonly IValue QUOTA = new Error("Resource limit exceeded");

            public static readonly IValue FLOAT = new Error("Floating-point arithmetic error");

            internal Error(string value)
            {
                this.Message = value;
            }

            public override ValueKind Kind => ValueKind.Error;

            public string Message { get; }

            public override IValue And(IValue value) =>
                new Value.Boolean(IsTruthy(this) && IsTruthy(value));

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

            public override IValue Not() =>
                new Value.Boolean(!IsTruthy(this));

            public override IValue Or(IValue value) =>
                new Value.Boolean(IsTruthy(this) || IsTruthy(value));

            public override string ToString() => this.Message;

            public override IValue Xor(IValue value) =>
                new Value.Boolean(IsTruthy(this) ^ IsTruthy(value));
        }
    }
}