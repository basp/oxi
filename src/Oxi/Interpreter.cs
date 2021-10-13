namespace Oxi
{
    using System;

    public class Interpreter : Expr.IVisitor<IValue>
    {
        public static string Stringify(IValue value)
        {
            if (value == null)
            {
                return "nil";
            }

            switch (value)
            {
                case Value.Boolean x:
                    return x.Value.ToString(Config.CultureInfo).ToLower();
                case Value.Float x:
                    return x.Value.ToString(Config.CultureInfo);
                default:
                    return value.ToString();
            }
        }

        public static bool AreEqual(object a, object b)
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

        public static bool IsThruthy(object value)
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

        public IValue Eval(Expr expr) => expr.Accept(this);

        public IValue VisitBinaryExpr(Expr.Binary expr)
        {
            var left = this.Eval(expr.Left);
            var right = this.Eval(expr.Right);

            switch (expr.Op)
            {
                case "==":
                    return new Value.Boolean(AreEqual(left, right));
                case "!=":
                    return new Value.Boolean(!AreEqual(left, right));
                case "-":
                    throw new NotImplementedException();
                case "/":
                    throw new NotImplementedException();
                case "*":
                    throw new NotImplementedException();
                case "<":
                    throw new NotImplementedException();
                case ">":
                    throw new NotImplementedException();
                case "<=":
                    throw new NotImplementedException();
                case ">=":
                    throw new NotImplementedException();
                case "+":
                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public IValue VisitGroupingExpr(Expr.Grouping expr) =>
            this.Eval(expr.Expression);

        public IValue VisitUnary(Expr.Unary expr)
        {
            var right = this.Eval(expr.Right);
            switch (expr.Op)
            {
                case "!":
                    return new Value.Boolean(!IsThruthy(right));
                case "-":
                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public IValue VisitLiteral(Expr.Literal expr) => expr.Value;
    }
}