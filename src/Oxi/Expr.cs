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

            T VisitIdentifier(Identifier expr);

            T VisitLiteral(Literal expr);

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

        public class Identifier : Expr
        {
            public Identifier(Token<TokenKind> tok, string value)
            {
                this.Token = tok;
                this.Value = value;
            }

            public string Value { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitIdentifier(this);
        }

        public class Literal : Expr
        {
            public Literal(Token<TokenKind> tok, IValue value)
            {
                this.Token = tok;
                this.Value = value;
            }

            public IValue Value { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitLiteral(this);
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

        public class VerbCall : Expr
        {
            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                throw new NotImplementedException();
            }
        }

        public class FunctionCall : Expr
        {
            public FunctionCall(Identifier fn, params Expr[] args)
            {
                this.Function = fn;
                this.Arguments = args;
            }

            public Identifier Function { get; }

            public Expr[] Arguments { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                throw new NotImplementedException();
            }
        }
    }
}
