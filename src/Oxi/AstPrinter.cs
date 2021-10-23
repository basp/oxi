namespace Oxi
{
    using System;
    using System.Linq;
    using System.Text;

    public class AstPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
    {
        private const string TILT = "<<< ?!?! TILT ?!?! >>>";

        private int level = 0;

        public string VisitBinary(Expr.Binary expr) =>
            this.Parenthesize(expr.Op, expr.Left, expr.Right);

        public string VisitBlock(Stmt.Block block)
        {
            var buf = new StringBuilder();
            buf.AppendLine(this.Indent("(block"));
            this.Indented(() =>
            {
                for (var i = 0; i < block.Body.Length - 1; i++)
                {
                    buf.AppendLine(block.Body[i].Accept(this));
                }

                buf.Append(block.Body[block.Body.Length - 1].Accept(this));
                buf.Append(")");
            });

            return buf.ToString();
        }

        public string VisitForStmt(Stmt.For stmt)
        {
            var buf = new StringBuilder();
            buf.AppendLine(this.Indent("(for"));
            this.Indented(() =>
            {
                buf.AppendLine(this.Indent(stmt.Condition.Accept(this)));
                buf.Append(stmt.Body.Accept(this));
                buf.Append(")");
            });

            return buf.ToString();
        }

        public string VisitIfStmt(Stmt.If stmt)
        {
            var buf = new StringBuilder();
            var arms = stmt.Conditions.Zip(
                stmt.Consequences,
                (cond, cons) => new { cond, cons })
                .ToArray();

            void WriteArm(Expr cond, Stmt cons)
            {
                buf.AppendLine(this.Indent("(arm"));
                this.Indented(() =>
                {
                    buf.AppendLine(this.Indent(cond.Accept(this)));
                    buf.Append(cons.Accept(this));
                });
            }

            buf.AppendLine(this.Indent("(if"));
            this.Indented(() =>
            {
                for (var i = 0; i < arms.Length - 1; i++)
                {
                    var arm = arms[i];
                    WriteArm(arm.cond, arm.cons);
                    buf.AppendLine(")");
                }

                var last = arms[arms.Length - 1];
                WriteArm(last.cond, last.cons);
                buf.Append(")");
            });
            buf.Append(")");
            return buf.ToString();
        }

        public string VisitExprStmt(Stmt.ExprStmt stmt) =>
            this.Indent(this.Parenthesize("stmt", stmt.Expression));

        public string VisitFunctionCall(Expr.FunctionCall expr)
        {
            var id = expr.Function.Accept(this);
            if (expr.Arguments.Length == 0)
            {
                return $"({id})";
            }

            var args = expr.Arguments
                .Select(x => x.Accept(this))
                .ToArray();

            return $"({id} {string.Join(" ", args)})";
        }

        public string VisitVerbCall(Expr.VerbCall call)
        {
            var obj = call.Object.Accept(this);
            var verb = call.Verb.Accept(this);
            if (call.Arguments.Length == 0)
            {
                return $"({obj}:{verb})";
            }

            var args = call.Arguments
                .Select(x => x.Accept(this))
                .ToArray();

            return $"({obj}:{verb} {string.Join(" ", args)})";
        }

        public string VisitGrouping(Expr.Grouping expr) =>
            this.Parenthesize("group", expr.Expression);

        public string VisitIdentifier(Expr.Identifier expr) =>
            expr.Value;

        public string VisitList(Expr.List expr)
        {
            var xs = expr.Elements.Select(x => x.Accept(this)).ToArray();
            return $"(list {string.Join(" ", xs)})";
        }

        public string VisitLiteral(Expr.Literal expr) =>
            expr.Value switch
            {
                Value.String x => x.ToString(),
                Value.Integer x => x.Value.ToString(Config.CultureInfo),
                Value.Float x => x.Value.ToString(Config.CultureInfo),
                Value.Boolean x => x.Value.ToString(Config.CultureInfo),
                Value.Object x => $"#{x.Value}",
                _ => TILT,
            };

        public string VisitReturn(Stmt.Return stmt) =>
            this.Indent($"(return {stmt.Expression.Accept(this)})");

        public string VisitUnary(Expr.Unary expr) =>
            this.Parenthesize(expr.Op, expr.Right);

        public string VisitRange(Expr.Range expr)
        {
            var buf = new StringBuilder();
            buf.Append($"(range ");
            buf.Append(expr.From.Accept(this));
            buf.Append(" ");
            buf.Append(expr.To.Accept(this));
            buf.Append(")");
            return buf.ToString();
        }

        public string VisitProperty(Expr.Property expr)
        {
            var obj = expr.Object.Accept(this);
            var name = expr.Name.Accept(this);
            return $"(prop {obj} {name})";
        }

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

        private void Indented(Action write)
        {
            this.level++;
            write();
            this.level--;
        }

        private string Indent(string value) =>
            string.Concat(string.Empty.PadLeft(this.level * 2), value);
    }
}