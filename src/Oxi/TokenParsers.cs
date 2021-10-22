namespace Oxi
{
    using System.Linq;
    using Superpower;
    using Superpower.Model;
    using Superpower.Parsers;

    public static class TokenParsers
    {
        public static readonly TokenListParser<TokenKind, Expr> Expr =
            Parse.Ref(() => Disjunction);

        public static readonly TokenListParser<TokenKind, Stmt> Program =
            Parse.Ref(() => Block);

        private static readonly TokenListParser<TokenKind, Stmt> Block =
            from stmts in Parse
                .OneOf(
                    Parse.Ref(() => ReturnStmt),
                    Parse.Ref(() => ExprStmt),
                    Parse.Ref(() => IfStmt))
                .AtLeastOnce()
            select (Stmt)new Stmt.Block(stmts[0].Token, stmts);

        private static readonly TokenListParser<TokenKind, Stmt> ExprStmt =
            from expr in Expr
            from semi in Token.EqualTo(TokenKind.Semicolon)
            select (Stmt)new Stmt.ExprStmt(expr.Token, expr);

        private static readonly TokenListParser<TokenKind, Stmt> ReturnStmt =
            from ret in Token.EqualTo(TokenKind.Return)
            from stmt in ExprStmt
            let expr = ((Stmt.ExprStmt)stmt).Expression
            select (Stmt)new Stmt.Return(ret, expr);

        private static TokenListParser<TokenKind, Conditional> IfThen(TokenKind kind) =>
            from @if in Token.EqualTo(kind)
            from cond in Parse.Ref(() => Grouping)
            from cons in Block
            select new Conditional(cond, cons);

        private static TokenListParser<TokenKind, Stmt> OptionalElse =
            (Token.EqualTo(TokenKind.Else).IgnoreThen(Block)).OptionalOrDefault();

        private static readonly TokenListParser<TokenKind, Stmt> IfStmt =
            from @if in IfThen(TokenKind.If)
            from elseIfs in IfThen(TokenKind.ElseIf).Many()
            from @else in OptionalElse
            from endif in Token.EqualTo(TokenKind.EndIf)
            let arms = new[] { @if }.Concat(elseIfs)
            let conditions = arms.Select(x => x.Condition).ToArray()
            let consequences = arms.Select(x => x.Consequence).ToArray()
            select (Stmt)new Stmt.If(conditions, consequences, @else);

        private static readonly TokenListParser<TokenKind, Stmt> KeywordStmt =
            ReturnStmt;

        private static readonly TokenListParser<TokenKind, Expr> True =
            Token
                .EqualTo(TokenKind.True)
                .Select(x => CreateBooleanLiteral(x, true));

        private static readonly TokenListParser<TokenKind, Expr> False =
            Token
                .EqualTo(TokenKind.False)
                .Select(x => CreateBooleanLiteral(x, false));

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Negate =
            Token.EqualTo(TokenKind.Minus);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Arrow =
            Token.EqualTo(TokenKind.EqualGreater);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Dot =
            Token.EqualTo(TokenKind.Dot);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Not =
            Token.EqualTo(TokenKind.Bang);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> And =
            Token.EqualTo(TokenKind.AmpersandAmpersand);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Or =
            Token.EqualTo(TokenKind.BarBar);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Eq =
            Token.EqualTo(TokenKind.EqualEqual);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Ne =
            Token.EqualTo(TokenKind.BangEqual);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Lt =
            Token.EqualTo(TokenKind.Less);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Gt =
            Token.EqualTo(TokenKind.Greater);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Ge =
            Token.EqualTo(TokenKind.GreaterEqual);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Le =
            Token.EqualTo(TokenKind.LessEqual);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Add =
            Token.EqualTo(TokenKind.Plus);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Subtract =
            Token.EqualTo(TokenKind.Minus);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Multiply =
            Token.EqualTo(TokenKind.Star);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Divide =
            Token.EqualTo(TokenKind.Slash);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Remainder =
            Token.EqualTo(TokenKind.Percent);

        private static readonly TokenListParser<TokenKind, Expr> Identifier =
            Token.EqualTo(TokenKind.Identifier)
                .Select(x => CreateIdentifier(x));

        private static readonly TokenListParser<TokenKind, Expr> Object =
            Token.EqualTo(TokenKind.Object)
                .Select(tok =>
                {
                    var str = tok.ToStringValue().TrimStart('#');
                    var i = int.Parse(str, Config.CultureInfo);
                    return CreateObjectLiteral(tok, i);
                });

        private static readonly TokenListParser<TokenKind, Expr> String =
            Token.EqualTo(TokenKind.String)
                .Select(x => new { tok = x, val = x.ToStringValue().Trim('"') })
                .Select(x => CreateStringLiteral(x.tok, x.val));

        private static readonly TokenListParser<TokenKind, Expr> Integer =
            Token.EqualTo(TokenKind.Integer)
                .Select(tok =>
                {
                    var str = tok.ToStringValue();
                    var i = int.Parse(str, Config.CultureInfo);
                    return CreateIntegerLiteral(tok, i);
                });

        private static readonly TokenListParser<TokenKind, Expr> Float =
            Token.EqualTo(TokenKind.Float)
                .Select(tok =>
                {
                    var str = tok.ToStringValue();
                    var f = double.Parse(str, Config.CultureInfo);
                    return CreateFloatLiteral(tok, f);
                });

        private static readonly TokenListParser<TokenKind, Expr> Literal =
            Parse.OneOf(
                Object,
                String,
                Integer,
                Float,
                True,
                False);

        private static readonly TokenListParser<TokenKind, Expr> List =
            from xs in Expr
                .ManyDelimitedBy(Token.EqualTo(TokenKind.Comma))
                .Between(
                    Token.EqualTo(TokenKind.LeftBrace),
                    Token.EqualTo(TokenKind.RightBrace))
            select (Expr)new Expr.List(Token<TokenKind>.Empty, xs);

        private static readonly TokenListParser<TokenKind, Expr> Grouping =
            from expr in Expr
                .Between(
                    Token.EqualTo(TokenKind.LeftParen),
                    Token.EqualTo(TokenKind.RightParen))
            select (Expr)new Expr.Grouping(expr.Token, expr);

        private static readonly TokenListParser<TokenKind, Expr> FunctionCall =
            from fn in Grouping.Or(Identifier)
            from args in Expr
                .ManyDelimitedBy(Token.EqualTo(TokenKind.Comma))
                .Between(
                    Token.EqualTo(TokenKind.LeftParen),
                    Token.EqualTo(TokenKind.RightParen))
            select (Expr)new Expr.FunctionCall(fn.Token, fn, args);

        private static readonly TokenListParser<TokenKind, Expr> VerbCall =
            from obj in Grouping.Or(Object).Or(Identifier)
            from colon in Token.EqualTo(TokenKind.Colon)
            from verb in Grouping.Or(Identifier)
            from args in Expr
                .ManyDelimitedBy(Token.EqualTo(TokenKind.Comma))
                .Between(
                    Token.EqualTo(TokenKind.LeftParen),
                    Token.EqualTo(TokenKind.RightParen))
            select (Expr)new Expr.VerbCall(obj.Token, obj, verb, args);

        private static readonly TokenListParser<TokenKind, Expr> Factor =
            Parse.OneOf(
                FunctionCall.Try(),
                VerbCall.Try(),
                List,
                Grouping,
                Literal,
                Identifier);

        private static readonly TokenListParser<TokenKind, Expr> Operand =
            (from op in Negate.Or(Not)
             from factor in Factor
             select CreateUnary(op, factor)).Or(Factor).Named("expression");

        private static readonly TokenListParser<TokenKind, Expr> Term =
            Parse.Chain(Dot.Or(Multiply).Or(Divide).Or(Remainder), Operand, CreateBinary);

        private static readonly TokenListParser<TokenKind, Expr> Comparand =
            Parse.Chain(Add.Or(Subtract), Term, CreateBinary);

        private static readonly TokenListParser<TokenKind, Expr> Comparison =
            Parse.Chain(Lt.Or(Ne).Or(Gt).Or(Eq), Comparand, CreateBinary);

        private static readonly TokenListParser<TokenKind, Expr> Conjunction =
            Parse.Chain(And, Comparison, CreateBinary);

        private static readonly TokenListParser<TokenKind, Expr> Disjunction =
            Parse.Chain(Or, Conjunction, CreateBinary);

        private static Expr CreateIdentifier(Token<TokenKind> tok) =>
            new Expr.Identifier(tok, tok.ToStringValue());

        private static Expr CreateObjectLiteral(Token<TokenKind> tok, int value) =>
            new Expr.Literal(tok, new Value.Object(value));

        private static Expr CreateIntegerLiteral(Token<TokenKind> tok, int value) =>
            new Expr.Literal(tok, new Value.Integer(value));

        private static Expr CreateFloatLiteral(Token<TokenKind> tok, double value) =>
            new Expr.Literal(tok, new Value.Float(value));

        private static Expr CreateStringLiteral(Token<TokenKind> tok, string value) =>
            new Expr.Literal(tok, new Value.String(value));

        private static Expr CreateBooleanLiteral(Token<TokenKind> tok, bool value) =>
            new Expr.Literal(tok, new Value.Boolean(value));

        private static Expr CreateBinary(Token<TokenKind> op, Expr left, Expr right) =>
            new Expr.Binary(op, left, op.ToStringValue(), right);

        private static Expr CreateUnary(Token<TokenKind> op, Expr right) =>
            new Expr.Unary(op, op.ToStringValue(), right);


        private class Conditional
        {
            public Conditional(Expr condition, Stmt consequence)
            {
                this.Condition = condition;
                this.Consequence = consequence;
            }

            public Expr Condition { get; }

            public Stmt Consequence { get; }
        }
    }
}