namespace Oxi
{
    using Superpower;
    using Superpower.Parsers;

    internal static class TokenParsers
    {
        private static readonly TokenListParser<TokenKind, Expr> String =
            Token.EqualTo(TokenKind.String)
                .Select(x => x.ToStringValue().Trim('"'))
                .Select(x => (Expr)new Expr.Literal(x));

        private static readonly TokenListParser<TokenKind, Expr> Number =
            Token.EqualTo(TokenKind.Number)
                .Select(x =>
                {
                    var str = x.ToStringValue();
                    if (int.TryParse(str, out var i))
                    {
                        return (Expr)new Expr.Literal(i);
                    }

                    return (Expr)new Expr.Literal(double.Parse(str));
                });
    }
}