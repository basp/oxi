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

            T VisitForStmt(For stmt);

            T VisitTryStmt(Stmt.Try stmt);
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

        public class Try : Stmt
        {
            public Try(Token<TokenKind> tok, Stmt body, Arm[] arms)
            {
                this.Token = tok;
                this.Body = body;
                this.Arms = arms;
            }

            public override Token<TokenKind> Token { get; }

            public Stmt Body { get; }

            public Try.Arm[] Arms { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitTryStmt(this);

            public class Arm
            {
                public Arm(Expr id, Expr[] errors, Stmt body)
                {
                    this.Identifier = id;
                    this.Errors = errors;
                    this.Body = body;
                }

                public Expr Identifier { get; }

                public Expr[] Errors { get; }

                public Stmt Body { get; }
            }
        }

        public class For : Stmt
        {
            public For(Token<TokenKind> tok, Expr id, Expr condition, Stmt body)
            {
                this.Token = tok;
                this.Id = id;
                this.Condition = condition;
                this.Body = body;
            }

            public Expr Id { get; }

            public Expr Condition { get; }

            public Stmt Body { get; }

            public override Token<TokenKind> Token { get; }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitForStmt(this);
        }

        public class If : Stmt
        {
            public If(
                Token<TokenKind> tok,
                Expr[] conditions,
                Stmt[] consequences,
                Stmt alternative = null)
            {
                this.Token = tok;
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

            public class Arm
            {
                public Arm(Expr condition, Stmt consequence)
                {
                    this.Condition = condition;
                    this.Consequence = consequence;
                }

                public Expr Condition { get; }

                public Stmt Consequence { get; }
            }
        }
    }
}