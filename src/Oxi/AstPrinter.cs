namespace Oxi
{
    using System.Text;

    public class AstPrinter : Expr.IVisitor<string>
    {
        public string VisitBinaryExpr(Expr.Binary expr) =>
            this.Parenthesize(expr.Op, expr.Left, expr.Right);

        public string VisitGroupingExpr(Expr.Grouping expr) =>
            this.Parenthesize("group", expr.Expression);

        public string VisitIntegerLiteral(Expr.IntegerLiteral expr) =>
            expr.Value.ToString();

        public string VisitFloatLiteral(Expr.FloatLiteral expr) =>
            expr.Value.ToString(Config.CultureInfo);

        public string VisitStringLiteral(Expr.StringLiteral expr) =>
            $"\"{expr.Value}\"";

        public string VisitBooleanLiteral(Expr.BooleanLiteral expr) =>
            expr.Value.ToString(Config.CultureInfo);

        public string VisitUnary(Expr.Unary expr) =>
            this.Parenthesize(expr.Op, expr.Right);

        private string Parenthesize(string name, params Expr[] exprs)
        {
            var buf = new StringBuilder();
            buf.Append("(");
            buf.Append(name);
            foreach (var expr in exprs)
            {
                buf.Append(" ");
                buf.Append(expr.Accept(this));
            }

            buf.Append(")");
            return buf.ToString();
        }
    }
}