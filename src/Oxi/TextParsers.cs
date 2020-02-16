namespace Oxi
{
    using Superpower;
    using Superpower.Parsers;

    internal static class TextParsers
    {
        public static readonly TextParser<TokenKind> CompoundOperator =
            EqualEqual.Or(
                GreaterEqual.Try().Or(
                LessEqual.Try().Or(BangEqual)));

        private static readonly TextParser<TokenKind> EqualEqual =
            Span.EqualTo("==").Value(TokenKind.EqualEqual);

        private static readonly TextParser<TokenKind> BangEqual =
            Span.EqualTo("!=").Value(TokenKind.BangEqual);

        private static readonly TextParser<TokenKind> LessEqual =
            Span.EqualTo("<=").Value(TokenKind.LessEqual);

        private static readonly TextParser<TokenKind> GreaterEqual =
            Span.EqualTo(">=").Value(TokenKind.GreaterEqual);
    }
}