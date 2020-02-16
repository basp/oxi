namespace Oxi
{
    using System;
    using Superpower.Model;

    public abstract class Expr
    {
        public interface IVisitor<T>
        {
            T VisitBinaryExpr(Binary expr);

            T VisitGroupingExpr(Grouping expr);

            T VisitLiteral(Literal expr);

            T VisitUnary(Unary expr);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Binary : Expr
        {
            public Binary(Expr left, string op, Expr right)
            {
                this.Left = left;
                this.Op = op;
                this.Right = right;
            }

            public Expr Left { get; }

            public string Op { get; }

            public Expr Right { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitBinaryExpr(this);
        }

        public class Grouping : Expr
        {
            public Grouping(Token<TokenKind> tok, Expr expr)
            {
                this.Expression = expr;
                this.Token = tok;
            }

            public Expr Expression { get; }

            public Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitGroupingExpr(this);
        }

        public class Literal : Expr
        {
            public Literal(Token<TokenKind> toke, object value)
            {
                this.Value = value;
                this.Token = toke;
            }

            public object Value { get; }

            public Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitLiteral(this);
        }

        public class Unary : Expr
        {
            public Unary(string op, Expr right)
            {
                this.Op = op;
                this.Right = right;
            }

            public string Op { get; }

            public Expr Right { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitUnary(this);
        }
    }
}
