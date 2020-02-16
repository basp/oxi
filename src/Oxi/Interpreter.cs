namespace Oxi
{
    public class Interpreter : Expr.IVisitor<object>
    {
        public object Eval(Expr expr) => expr.Accept(this);

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            throw new System.NotImplementedException();
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