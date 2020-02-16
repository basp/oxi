namespace Oxi
{
    using System;

    public class Interpreter : Expr.IVisitor<object>
    {
        public object Eval(Expr expr) => expr.Accept(this);

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = this.Eval(expr.Left);
            var right = this.Eval(expr.Right);
            switch (expr.Op)
            {
                case "==":
                    return AreEqual(left, right);
                case "!=":
                    return !AreEqual(left, right);
                case "-":
                    return (double)left - (double)right;
                case "/":
                    return (double)left / (double)right;
                case "*":
                    return (double)left * (double)right;
                case "<":
                    return (double)left < (double)right;
                case ">":
                    return (double)left > (double)right;
                case "<=":
                    return (double)left <= (double)right;
                case ">=":
                    return (double)left >= (double)right;
                case "+":
                    if (left is string && right is string)
                    {
                        return string.Concat(left, right);
                    }

                    if (left is string)
                    {
                        return string.Concat(left, Stringify(right));
                    }

                    if (right is string)
                    {
                        return string.Concat(Stringify(left), right);
                    }

                    return (double)left + (double)right;
            }

            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Expr.Grouping expr) =>
            this.Eval(expr.Expression);

        public object VisitLiteral(Expr.Literal expr) => expr.Value;

        public object VisitUnary(Expr.Unary expr)
        {
            var right = this.Eval(expr.Right);
            switch (expr.Op)
            {
                case "!":
                    return !IsThruthy(right);
                case "-":
                    return -(double)right;
            }

            return null;
        }

        public static string Stringify(object value)
        {
            if (value == null)
            {
                return "nil";
            }

            switch (value)
            {
                case bool x: return x.ToString().ToLower();
                case double x: return x.ToString();
                default: return value.ToString();
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
    }
}