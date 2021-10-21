namespace Oxi
{
    public class PrettyPrinter : Expr.IVisitor<string>
    {
        public string VisitBinaryExpr(Expr.Binary expr)
        {
            var left = expr.Left.Accept(this);
            var right = expr.Right.Accept(this);
            return $"{left} {expr.Op} {right}";
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            var body = expr.Expression.Accept(this);
            return $"({body})";
        }

        public string VisitIdentifier(Expr.Identifier expr) => expr.Value;

        public string VisitLiteral(Expr.Literal expr) => expr.Value.ToString();

        public string VisitUnary(Expr.Unary expr)
        {
            var right = expr.Right.Accept(this);
            return $"{expr.Op}{right}";
        }
    }
}