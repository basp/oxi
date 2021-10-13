namespace Oxi
{
    using System.Globalization;
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
                .Select(x => CreateBooleanLiteral(x, true));

        private static readonly TokenListParser<TokenKind, Expr> False =
            Token
                .EqualTo(TokenKind.False)
                .Select(x => CreateBooleanLiteral(x, false));

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Negate =
            Token.EqualTo(TokenKind.Minus);

        private static readonly TokenListParser<TokenKind, Token<TokenKind>> Arrow =
            Token.EqualTo(TokenKind.EqualGreater);

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
            String
                .Or(Integer)
                .Or(Float)
                .Or(True)
                .Or(False);

        private static readonly TokenListParser<TokenKind, Expr> Factor =
            (from lparen in Token.EqualTo(TokenKind.LeftParen)
             from expr in Parse.Ref(() => Expr)
             from rparen in Token.EqualTo(TokenKind.RightParen)
             select (Expr)new Expr.Grouping(lparen, expr)).Or(Literal);

        private static readonly TokenListParser<TokenKind, Expr> Operand =
            (from op in Negate.Or(Not)
             from factor in Factor
             select CreateUnary(op, factor)).Or(Factor).Named("expression");

        private static readonly TokenListParser<TokenKind, Expr> Term =
            Parse.Chain(Multiply.Or(Divide), Operand, CreateBinary);

        private static readonly TokenListParser<TokenKind, Expr> Comparand =
            Parse.Chain(Add.Or(Subtract), Term, CreateBinary);

        private static readonly TokenListParser<TokenKind, Expr> Comparison =
            Parse.Chain(Lt.Or(Ne).Or(Gt).Or(Eq), Comparand, CreateBinary);

        private static readonly TokenListParser<TokenKind, Expr> Conjunction =
            Parse.Chain(And, Comparison, CreateBinary);

        private static readonly TokenListParser<TokenKind, Expr> Disjunction =
            Parse.Chain(Or, Conjunction, CreateBinary);

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
    }
}