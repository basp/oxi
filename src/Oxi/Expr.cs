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

            T VisitStringLiteral(StringLiteral expr);

            T VisitIntegerLiteral(IntegerLiteral expr);

            T VisitFloatLiteral(FloatLiteral expr);

            T VisitBooleanLiteral(BooleanLiteral expr);

            T VisitUnary(Unary expr);
        }

        public abstract Token<TokenKind> Token { get; }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Binary : Expr
        {
            public Binary(
                Token<TokenKind> tok,
                Expr left,
                string op,
                Expr right)
            {
                this.Token = tok;
                this.Left = left;
                this.Op = op;
                this.Right = right;
            }

            public Expr Left { get; }

            public string Op { get; }

            public Expr Right { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitBinaryExpr(this);
        }

        public class Grouping : Expr
        {
            public Grouping(Token<TokenKind> tok, Expr expr)
            {
                this.Token = tok;
                this.Expression = expr;
            }

            public Expr Expression { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitGroupingExpr(this);
        }

        public abstract class Literal<T> : Expr
        {
            protected Literal(Token<TokenKind> tok, T value)
            {
                this.Token = tok;
                this.Value = value;
            }

            public T Value { get; }

            public override Token<TokenKind> Token { get; }
        }

        public class BooleanLiteral : Literal<bool>
        {
            public BooleanLiteral(Token<TokenKind> token, bool value)
                : base(token, value)
            {
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitBooleanLiteral(this);
        }

        public class StringLiteral : Literal<string>
        {
            public StringLiteral(Token<TokenKind> token, string value)
                : base(token, value)
            {
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitStringLiteral(this);
        }

        public class IntegerLiteral : Literal<int>
        {
            public IntegerLiteral(Token<TokenKind> token, int value)
                : base(token, value)
            {
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitIntegerLiteral(this);
        }

        public class FloatLiteral : Literal<double>
        {
            public FloatLiteral(Token<TokenKind> token, double value)
                : base(token, value)
            {
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitFloatLiteral(this);
        }

        public class Unary : Expr
        {
            public Unary(Token<TokenKind> tok, string op, Expr right)
            {
                this.Token = tok;
                this.Op = op;
                this.Right = right;
            }

            public string Op { get; }

            public Expr Right { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitUnary(this);
        }
    }
}
