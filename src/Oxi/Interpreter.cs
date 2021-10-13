namespace Oxi
{
    using System;

    public class Interpreter : Expr.IVisitor<object>
    {
        public static string Stringify(object value)
        {
            if (value == null)
            {
                return "nil";
            }

            switch (value)
            {
                case bool x: 
                    return x.ToString(Config.CultureInfo).ToLower();
                case double x: 
                    return x.ToString(Config.CultureInfo);
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

        public object Eval(Expr expr) => expr.Accept(this);

        public object VisitStringLiteral(Expr.StringLiteral expr) =>
            expr.Value;

        public object VisitIntegerLiteral(Expr.IntegerLiteral expr) =>
            expr.Value;

        public object VisitFloatLiteral(Expr.FloatLiteral expr) =>
            expr.Value;

        public object VisitBooleanLiteral(Expr.BooleanLiteral expr) =>
            expr.Value;

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
                    if (left is string || right is string)
                    {
                        return string.Concat(
                            Stringify(left),
                            Stringify(right));
                    }

                    if (left is double || right is double)
                    {
                        return
                            Convert.ToDouble(left) +
                            Convert.ToDouble(right);
                    }

                    if (left is int && right is int)
                    {
                        return (int)left + (int)right;
                    }

                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Expr.Grouping expr) =>
            this.Eval(expr.Expression);

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
    }
}