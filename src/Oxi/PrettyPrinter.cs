namespace Oxi
{
    using System;
    using System.Linq;
    using System.Text;

    public class PrettyPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
    {
        private int level = 0;

        public string VisitBinary(Expr.Binary expr)
        {
            var left = expr.Left.Accept(this);
            var right = expr.Right.Accept(this);
            return $"{left} {expr.Op} {right}";
        }

        public string VisitBlock(Stmt.Block block)
        {
            var buf = new StringBuilder();
            for (var i = 0; i < block.Body.Length - 1; i++)
            {
                var stmt = block.Body[i];
                buf.AppendLine(stmt.Accept(this));
            }

            var last = block.Body[block.Body.Length - 1].Accept(this);
            if (AtTopLevel)
            {
                // Top level block does not need line ending
                buf.Append(last);
            }
            else
            {
                buf.AppendLine(last);
            }

            return buf.ToString();
        }

        public string VisitExprStmt(Stmt.ExprStmt stmt) =>
            Indent($"{stmt.Expression.Accept(this)};");

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
            Indent($"return {stmt.Expression.Accept(this)};");

        public string VisitUnary(Expr.Unary expr)
        {
            var right = expr.Right.Accept(this);
            return $"{expr.Op}{right}";
        }

        public string VisitIfStmt(Stmt.If stmt)
        {
            var arms = stmt.Conditions.Zip(
                stmt.Consequences,
                (cond, cons) => new { cond, cons });
            var head = arms.First();
            var tail = arms.Skip(1);
            var buf = new StringBuilder();
            buf.AppendLine(Indent($"if {head.cond.Accept(this)}"));
            Indented(() => buf.Append(head.cons.Accept(this)));
            buf.Append(Indent("endif"));
            return buf.ToString();
        }

        private bool AtTopLevel => this.level == 0;

        private void Indented(Action write)
        {
            this.level++;
            write();
            this.level--;
        }

        private string Indent(string value)
        {
            return string.Concat("".PadLeft(this.level * 2), value);
        }
    }
}