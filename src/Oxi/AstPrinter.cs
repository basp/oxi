namespace Oxi
{
    using System;
    using System.Text;

    public class AstPrinter : Expr.IVisitor<string>
    {
        public string VisitBinaryExpr(Expr.Binary expr) =>
            this.Parenthesize(expr.Op, expr.Left, expr.Right);

        public string VisitGroupingExpr(Expr.Grouping expr) =>
            this.Parenthesize("group", expr.Expression);

        public string VisitLiteral(Expr.Literal expr)
        {
            throw new NotImplementedException();
        }

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