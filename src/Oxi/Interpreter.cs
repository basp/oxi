namespace Oxi
{
    using System;
    using System.Linq;
    using Superpower.Model;

    public class Interpreter : Expr.IVisitor<IValue>, Stmt.IVisitor<IValue>
    {
        public IValue Exec(Stmt stmt) =>
            ThrowIfError(stmt.Accept(this), stmt.Token.Position);

        public IValue Eval(Expr expr) =>
            ThrowIfError(expr.Accept(this), expr.Token.Position);

        public IValue VisitExprStmt(Stmt.ExprStmt stmt) =>
            ThrowIfError(
                this.Eval(stmt.Expression),
                stmt.Token.Position);

        public IValue VisitBlock(Stmt.Block block)
        {
            IValue result = null;
            foreach (var stmt in block.Body)
            {
                result = ThrowIfError(stmt.Accept(this), stmt.Token.Position);
            }

            return result;
        }

        public IValue VisitReturn(Stmt.Return stmt) =>
            this.Eval(stmt.Expression);

        public IValue VisitIfStmt(Stmt.If stmt) =>
            throw new NotImplementedException();

        public IValue VisitBinary(Expr.Binary expr)
        {
            var left = ThrowIfError(
                this.Eval(expr.Left),
                expr.Left.Token.Position);

            var right = ThrowIfError(
                this.Eval(expr.Right),
                expr.Right.Token.Position);

            IValue result = expr.Op switch
            {
                "==" => new Value.Boolean(AreEqual(left, right)),
                "!=" => new Value.Boolean(AreNotEqual(left, right)),
                "<" => throw new NotImplementedException(),
                ">" => throw new NotImplementedException(),
                "<=" => throw new NotImplementedException(),
                ">=" => throw new NotImplementedException(),
                "-" => Subtract(left, right),
                "+" => Add(left, right),
                "/" => Divide(left, right),
                "%" => Remainder(left, right),
                "*" => Multiply(left, right),
                _ => throw new RuntimeException(
                    $"invalid binary operation `{expr.Op}`",
                    expr.Token.Position),
            };

            return ThrowIfError(result, expr.Token.Position);
        }

        public IValue VisitGrouping(Expr.Grouping expr) =>
            this.Eval(expr.Expression);

        public IValue VisitUnary(Expr.Unary expr)
        {
            var right = ThrowIfError(
                this.Eval(expr.Right),
                expr.Right.Token.Position);

            IValue result = expr.Op switch
            {
                "!" => new Value.Boolean(!IsThruthy(right)),
                "-" => throw new NotImplementedException(),
                _ => throw new RuntimeException(
                    $"invalid unary operation `{expr.Op}`",
                    expr.Token.Position),
            };

            return ThrowIfError(result, expr.Token.Position);
        }

        public IValue VisitList(Expr.List expr)
        {
            var xs = expr.Elements.Select(x => x.Accept(this)).ToArray();
            return new Value.List(xs);
        }

        public IValue VisitIdentifier(Expr.Identifier expr) =>
            Value.Error.VARNF;

        public IValue VisitLiteral(Expr.Literal expr) => expr.Value;

        public IValue VisitFunctionCall(Expr.FunctionCall expr)
        {
            return new Value.Boolean(true);
        }

        public IValue VisitVerbCall(Expr.VerbCall expr)
        {
            return new Value.Boolean(true);
        }

        private static IValue ThrowIfError(IValue value, Position position)
        {
            if (value is Value.Error err)
            {
                throw new RuntimeException(err.Message, position);
            }

            return value;
        }

        private static IValue Add(IValue left, IValue right) =>
            (left, right) switch
            {
                (IFloatable x, IFloatable y) => x.Add(y),
                (Value.String x, Value.String y) =>
                    new Value.String(x.Value + y.Value),
                (Value.String x, IValue y) =>
                    new Value.String(x.Value + y.ToString()),
                (IValue x, Value.String y) =>
                    new Value.String(x.ToString() + y.Value),
                _ => Value.Error.TYPE,
            };

        private static IValue Subtract(IValue left, IValue right) =>
            (left, right) switch
            {
                (IFloatable x, IFloatable y) => x.Sub(y),
                _ => Value.Error.TYPE,
            };

        private static IValue Multiply(IValue left, IValue right) =>
            (left, right) switch
            {
                (IFloatable x, IFloatable y) => x.Mul(y),
                _ => Value.Error.TYPE,
            };

        private static IValue Divide(IValue left, IValue right) =>
            (left, right) switch
            {
                (IFloatable x, IFloatable y) => x.Divide(y),
                _ => Value.Error.TYPE,
            };

        private static IValue Remainder(IValue left, IValue right) =>
            (left, right) switch
            {
                (IFloatable x, IFloatable y) => x.Rem(y),
                _ => Value.Error.TYPE,
            };

        private static bool AreEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private static bool AreNotEqual(object a, object b) => !AreEqual(a, b);

        private static IValue Compare(IValue left, IValue right) =>
            (left, right) switch
            {
                (IOrdinal x, IOrdinal y) => new Value.Integer(x.CompareTo(y)),
                _ => Value.Error.TYPE,
            };

        private static bool IsThruthy(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool)
            {
                return (bool)value;
            }

            return true;
        }
    }
}