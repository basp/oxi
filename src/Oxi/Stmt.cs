namespace Oxi
{
    using Superpower.Model;

    public abstract class Stmt
    {
        public interface IVisitor<T>
        {
            T VisitExprStmt(ExprStmt stmt);

            T VisitBlock(Block stmt);

            T VisitReturn(Return stmt);

            T VisitIfStmt(If stmt);
        }

        public abstract Token<TokenKind> Token { get; }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class ExprStmt : Stmt
        {
            public ExprStmt(Token<TokenKind> tok, Expr expr)
            {
                this.Token = tok;
                this.Expression = expr;
            }

            public Expr Expression { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitExprStmt(this);
        }

        public class Block : Stmt
        {
            public Block(Token<TokenKind> tok, Stmt[] body)
            {
                this.Token = tok;
                this.Body = body;
            }

            public Stmt[] Body { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitBlock(this);
        }

        public class Return : Stmt
        {
            public Return(Token<TokenKind> tok, Expr expr)
            {
                this.Token = tok;
                this.Expression = expr;
            }

            public Expr Expression { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitReturn(this);
        }

        public class If : Stmt
        {
            public If(
                Expr[] conditions,
                Stmt[] consequences,
                Stmt alternative = null)
            {
                this.Conditions = conditions;
                this.Consequences = consequences;
                this.Alternative = alternative;
            }

            public Expr[] Conditions { get; }

            public Stmt[] Consequences { get; }

            public Stmt Alternative { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitIfStmt(this);
        }
    }
}