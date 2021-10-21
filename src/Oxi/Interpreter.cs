namespace Oxi
{
    using System;
    using Superpower.Model;

    public class Interpreter : Expr.IVisitor<IValue>
    {
        public IValue Eval(Expr expr) =>
            ThrowIfError(expr.Accept(this), expr.Token.Position);

        public IValue VisitBinaryExpr(Expr.Binary expr)
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
                    $"Invalid binary operation `{expr.Op}`",
                    expr.Token.Position),
            };

            return ThrowIfError(result, expr.Token.Position);
        }

        public IValue VisitGroupingExpr(Expr.Grouping expr) =>
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
                    $"Invalid unary operation `{expr.Op}`",
                    expr.Token.Position),
            };

            return ThrowIfError(result, expr.Token.Position);
        }

        public IValue VisitIdentifier(Expr.Identifier expr) =>
            Value.Error.VARNF;

        public IValue VisitLiteral(Expr.Literal expr) => expr.Value;

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