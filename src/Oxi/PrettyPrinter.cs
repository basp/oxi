namespace Oxi;

using System;
using System.Linq;
using System.Text;

public class PrettyPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
{
    private int level = 0;

    private bool AtTopLevel => this.level == 0;

    public string VisitBinary(Expr.Binary expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);

        if (expr.Op == ".")
        {
            return $"{left}.{right}";
        }

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
        if (this.AtTopLevel)
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
        this.Indent($"{stmt.Expression.Accept(this)};");

    public string VisitFunctionCall(Expr.FunctionCall expr)
    {
        var fn = expr.Function.Accept(this);
        var args = expr.Arguments.Select(x => x.Accept(this));
        return $"{fn}({string.Join(", ", args)})";
    }

    public string VisitVerbCall(Expr.VerbCall call)
    {
        var obj = call.Object.Accept(this);
        var verb = call.Verb.Accept(this);
        var args = call.Arguments.Select(x => x.Accept(this));
        return $"{obj}:{verb}({string.Join(", ", args)})";
    }

    public string VisitGrouping(Expr.Grouping expr)
    {
        var body = expr.Expression.Accept(this);
        return $"({body})";
    }

    public string VisitList(Expr.List expr)
    {
        var xs = expr.Elements.Select(x => x.Accept(this));
        return $"{{{string.Join(", ", xs)}}}";
    }

    public string VisitIdentifier(Expr.Identifier expr) => expr.Value;

    public string VisitLiteral(Expr.Literal expr) => expr.Value.ToString();

    public string VisitReturn(Stmt.Return stmt) =>
        this.Indent($"return {stmt.Expression.Accept(this)};");

    public string VisitUnary(Expr.Unary expr)
    {
        var right = expr.Right.Accept(this);
        return $"{expr.Op}{right}";
    }

    public string VisitRange(Expr.Range expr)
    {
        var from = expr.From.Accept(this);
        var to = expr.To.Accept(this);
        return $"[{from}..{to}]";
    }

    public string VisitIfStmt(Stmt.If stmt)
    {
        var arms = stmt.Conditions.Zip(
            stmt.Consequences,
            (cond, cons) => new { cond, cons });
        var head = arms.First();
        var tail = arms.Skip(1);
        var buf = new StringBuilder();
        buf.AppendLine(this.Indent($"if {head.cond.Accept(this)}"));
        this.Indented(() => buf.Append(head.cons.Accept(this)));
        foreach (var arm in tail)
        {
            buf.AppendLine(this.Indent($"elseif {arm.cond.Accept(this)}"));
            this.Indented(() => buf.Append(arm.cons.Accept(this)));
        }

        if (stmt.Alternative != null)
        {
            buf.AppendLine(this.Indent("else"));
            this.Indented(() => buf.Append(stmt.Alternative.Accept(this)));
        }

        buf.Append(this.Indent("endif"));
        return buf.ToString();
    }

    public string VisitForStmt(Stmt.For stmt)
    {
        var buf = new StringBuilder();
        var cond = stmt.Condition.Accept(this);
        var id = stmt.Id.Accept(this);
        buf.AppendLine(this.Indent($"for {id} in {cond}"));
        this.Indented(() => buf.Append(stmt.Body.Accept(this)));
        buf.Append(this.Indent("endfor"));
        return buf.ToString();
    }

    public string VisitTryExpr(Expr.Try expr)
    {
        throw new NotImplementedException();
    }

    public string VisitTryStmt(Stmt.Try stmt)
    {
        throw new NotImplementedException();
    }

    public string VisitProperty(Expr.Property expr)
    {
        var obj = expr.Object.Accept(this);
        var name = expr.Name.Accept(this);
        return $"{obj}.{name}";
    }

    private void Indented(Action write)
    {
        this.level++;
        write();
        this.level--;
    }

    private string Indent(string value)
    {
        return string.Concat(
            string.Empty.PadLeft(this.level * 2),
            value);
    }
}
