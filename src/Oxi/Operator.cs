namespace Oxi;

public class Operator
{
    public static readonly Operator Not = new Operator("!");

    public static readonly Operator Neg = new Operator("-");

    public static readonly Operator Add = new Operator("+");

    public static readonly Operator Sub = new Operator("-");

    public static readonly Operator Div = new Operator("/");

    public static readonly Operator Mul = new Operator("*");

    public static readonly Operator Eq = new Operator("==");

    public static readonly Operator Ne = new Operator("!=");

    public static readonly Operator Lt = new Operator("<");

    public static readonly Operator Gt = new Operator(">");

    public static readonly Operator Le = new Operator("<=");

    public static readonly Operator Ge = new Operator(">=");

    public static readonly Operator And = new Operator("and");

    public static readonly Operator Or = new Operator("or");

    private Operator(string name)
    {
        this.Name = name;
    }

    public string Name { get; }

    public override bool Equals(object obj)
    {
        if (object.ReferenceEquals(this, obj))
        {
            return true;
        }

        var other = obj as Operator;
        if (other == null)
        {
            return false;
        }

        return this.Name == other.Name;
    }

    public override int GetHashCode() => this.Name.GetHashCode();
}
