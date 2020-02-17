namespace Oxi
{
    using Superpower;
    using Superpower.Model;
    using Superpower.Parsers;

    public static class TokenParsers
    {
        public static readonly TokenListParser<TokenKind, Expr> Expr =
            Parse.Ref(() => Disjunction);

        private static readonly TokenListParser<TokenKind, Expr> True =
            Token
                .EqualTo(TokenKind.True)
                .Select(x => (Expr)new Expr.Literal(x, true));

        private static readonly TokenListParser<TokenKind, Expr> False =
            Token
                .EqualTo(TokenKind.False)
                .Select(x => (Expr)new Expr.Literal(x, false));

        private static readonly TokenListParser<TokenKind, Expr> Nil =
            Token
                .EqualTo(TokenKind.Nil)
                .Select(x => (Expr)new Expr.Literal(x, null));

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Neg =
            Token.EqualTo(TokenKind.Minus);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Not =
            Token.EqualTo(TokenKind.Bang);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> And =
            Token.EqualTo(TokenKind.And);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Or =
            Token.EqualTo(TokenKind.Or);

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

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Sub =
            Token.EqualTo(TokenKind.Minus);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Mul =
            Token.EqualTo(TokenKind.Star);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Div =
            Token.EqualTo(TokenKind.Slash);

        private static readonly TokenListParser<TokenKind, Expr> String =
            Token.EqualTo(TokenKind.String)
                .Select(x => new { tok = x, val = x.ToStringValue().Trim('"') })
                .Select(x => (Expr)new Expr.Literal(x.tok, x.val));

        private static readonly TokenListParser<TokenKind, Expr> Number =
            Token.EqualTo(TokenKind.Number)
                .Select(x =>
                {
                    var str = x.ToStringValue();
                    var f = double.Parse(str);
                    return (Expr)new Expr.Literal(x, f);
                });

        private static readonly TokenListParser<TokenKind, Expr> Literal =
            String
                .Or(Number)
                .Or(True)
                .Or(False)
                .Or(Nil);

        private static readonly TokenListParser<TokenKind, Expr> Factor =
            (from lparen in Token.EqualTo(TokenKind.LeftParen)
             from expr in Parse.Ref(() => Expr)
             from rparen in Token.EqualTo(TokenKind.RightParen)
             select (Expr)new Expr.Grouping(lparen, expr)).Or(Literal);

        private static readonly TokenListParser<TokenKind, Expr> Operand =
            (from op in Neg.Or(Not)
             from factor in Factor
             select MakeUnary(op, factor)).Or(Factor).Named("expression");

        private static readonly TokenListParser<TokenKind, Expr> Term =
            Parse.Chain(Mul.Or(Div), Operand, MakeBinary);

        private static readonly TokenListParser<TokenKind, Expr> Comparand =
            Parse.Chain(Add.Or(Sub), Term, MakeBinary);

        private static readonly TokenListParser<TokenKind, Expr> Comparison =
            Parse.Chain(Lt.Or(Ne).Or(Gt).Or(Eq), Comparand, MakeBinary);

        private static readonly TokenListParser<TokenKind, Expr> Conjunction =
            Parse.Chain(And, Comparison, MakeBinary);

        private static readonly TokenListParser<TokenKind, Expr> Disjunction =
            Parse.Chain(Or, Conjunction, MakeBinary);

        private static Expr MakeBinary(Token<TokenKind> op, Expr left, Expr right) =>
            new Expr.Binary(op, left, op.ToStringValue(), right);

        private static Expr MakeUnary(Token<TokenKind> op, Expr right) =>
            new Expr.Unary(op, op.ToStringValue(), right);
    }
}