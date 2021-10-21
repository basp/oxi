namespace Oxi
{
    using System.Text;

    public class AstPrinter : Expr.IVisitor<string>
    {
        public string VisitBinaryExpr(Expr.Binary expr) =>
            this.Parenthesize(expr.Op, expr.Left, expr.Right);

        public string VisitGroupingExpr(Expr.Grouping expr) =>
            this.Parenthesize("group", expr.Expression);

        public string VisitIdentifier(Expr.Identifier expr) =>
            expr.Value;

        public string VisitLiteral(Expr.Literal expr) =>
            expr.Value switch
            {
                Value.String x => x.ToString(),
                Value.Integer x => x.Value.ToString(Config.CultureInfo),
                Value.Float x => x.Value.ToString(Config.CultureInfo),
                Value.Boolean x => x.Value.ToString(Config.CultureInfo),
                Value.Object x => $"#{x.Value}",
                _ => "TILT",
            };

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