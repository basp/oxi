namespace Oxi
{
    using Superpower;
    using Superpower.Model;
    using Superpower.Parsers;
    using Superpower.Tokenizers;

    public class Scanner
    {
        private static readonly Tokenizer<TokenKind> Tokenizer =
            new TokenizerBuilder<TokenKind>()
                .Ignore(Span.WhiteSpace)
                .Match(Span.EqualTo("=="), TokenKind.EqualEqual)
                .Match(Span.EqualTo("!="), TokenKind.BangEqual)
                .Match(Span.EqualTo("<="), TokenKind.LessEqual)
                .Match(Span.EqualTo(">="), TokenKind.GreaterEqual)
                .Match(Character.EqualTo('='), TokenKind.Equal)
                .Match(Character.EqualTo('+'), TokenKind.Plus)
                .Match(Character.EqualTo('-'), TokenKind.Minus)
                .Match(Character.EqualTo('!'), TokenKind.Bang)
                .Match(Character.EqualTo('/'), TokenKind.Slash)
                .Match(Character.EqualTo('*'), TokenKind.Star)
                .Match(Character.EqualTo('<'), TokenKind.Less)
                .Match(Character.EqualTo('>'), TokenKind.Greater)
                .Match(Character.EqualTo(';'), TokenKind.Semicolon)
                .Match(Character.EqualTo(','), TokenKind.Comma)
                .Match(Character.EqualTo('('), TokenKind.LeftParen)
                .Match(Character.EqualTo(')'), TokenKind.RightParen)
                .Match(Character.EqualTo('{'), TokenKind.LeftBrace)
                .Match(Character.EqualTo('}'), TokenKind.RightBrace)
                .Match(Span.EqualTo("print"), TokenKind.Print)
                .Match(Span.EqualTo("let"), TokenKind.Var)
                .Match(Span.EqualTo("true"), TokenKind.True)
                .Match(Span.EqualTo("false"), TokenKind.False)
                .Match(Span.EqualTo("nil"), TokenKind.Nil)
                .Match(Span.EqualTo("if"), TokenKind.If)
                .Match(Span.EqualTo("else"), TokenKind.Else)
                .Match(Span.EqualTo("return"), TokenKind.Return)
                .Match(Span.EqualTo("and"), TokenKind.And)
                .Match(Span.EqualTo("or"), TokenKind.Or)
                .Match(Numerics.Decimal, TokenKind.Number)
                .Match(QuotedString.CStyle, TokenKind.String)
                .Match(Identifier.CStyle, TokenKind.Identifier)
                .Build();

        public TokenList<TokenKind> Tokenize(string source) =>
            Tokenizer.Tokenize(source);
    }
}