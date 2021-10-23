namespace Oxi
{
    using Superpower;
    using Superpower.Parsers;

    internal static class TextParsers
    {
        public static readonly TextParser<TokenKind> DotDot =
            Span.EqualTo("..").Value(TokenKind.DotDot);

        public static readonly TextParser<TokenKind> EqualEqual =
            Span.EqualTo("==").Value(TokenKind.EqualEqual);

        public static readonly TextParser<TokenKind> BangEqual =
            Span.EqualTo("!=").Value(TokenKind.BangEqual);

        public static readonly TextParser<TokenKind> LessEqual =
            Span.EqualTo("<=").Value(TokenKind.LessEqual);

        public static readonly TextParser<TokenKind> GreaterEqual =
            Span.EqualTo(">=").Value(TokenKind.GreaterEqual);

        public static readonly TextParser<TokenKind> Arrow =
            Span.EqualTo("=>").Value(TokenKind.EqualGreater);

        public static readonly TextParser<TokenKind> CompoundOperator =
            Parse.OneOf(
                Arrow.Try(),
                EqualEqual,
                DotDot,
                GreaterEqual,
                LessEqual,
                BangEqual);
    }
}