namespace Oxi
{
    using System;
    using System.Text;

    public class PrettyPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
    {
        public string VisitBinary(Expr.Binary expr)
        {
            var left = expr.Left.Accept(this);
            var right = expr.Right.Accept(this);
            return $"{left} {expr.Op} {right}";
        }

        public string VisitBlock(Stmt.Block block)
        {
            var buf = new StringBuilder();
            Array.ForEach(
                block.Body,
                stmt => buf.AppendLine(stmt.Accept(this)));
            return buf.ToString();
        }

        public string VisitExprStmt(Stmt.ExprStmt stmt) =>
            $"{stmt.Expression.Accept(this)};";

        public string VisitFunctionCall(Expr.FunctionCall expr)
        {
            throw new NotImplementedException();
        }

        public string VisitVerbCall(Expr.VerbCall call)
        {
            throw new NotImplementedException();
        }

        public string VisitGrouping(Expr.Grouping expr)
        {
            var body = expr.Expression.Accept(this);
            return $"({body})";
        }

        public string VisitList(Expr.List expr)
        {
            throw new NotImplementedException();
        }

        public string VisitIdentifier(Expr.Identifier expr) => expr.Value;

        public string VisitLiteral(Expr.Literal expr) => expr.Value.ToString();

        public string VisitReturn(Stmt.Return stmt) =>
            $"return {stmt.Expression.Accept(this)};";

        public string VisitUnary(Expr.Unary expr)
        {
            var right = expr.Right.Accept(this);
            return $"{expr.Op}{right}";
        }
    }
}