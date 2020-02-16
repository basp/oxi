namespace Oxi
{
    using Superpower;
    using Superpower.Parsers;

    public static class TokenParsers
    {
        // public static readonly TokenListParser<TokenKind, Expr> Identifier =
        //     Token.EqualTo(TokenKind.Identifier).Select(x => new Expr.)

        public static readonly TokenListParser<TokenKind, Expr> True =
            Token.EqualTo(TokenKind.True).Value((Expr)new Expr.Literal(true));

        public static readonly TokenListParser<TokenKind, Expr> False =
            Token.EqualTo(TokenKind.False).Value((Expr)new Expr.Literal(false));

        public static readonly TokenListParser<TokenKind, Expr> Nil =
            Token.EqualTo(TokenKind.Nil).Value((Expr)new Expr.Literal(null));

        public static readonly TokenListParser<TokenKind, string> Negate =
            Token.EqualTo(TokenKind.Minus).Value("-");

        public static readonly TokenListParser<TokenKind, string> Not =
            Token.EqualTo(TokenKind.BangEqual).Value("!");

        public static readonly TokenListParser<TokenKind, string> And =
            Token.EqualTo(TokenKind.And).Value("and");

        public static readonly TokenListParser<TokenKind, string> Or =
            Token.EqualTo(TokenKind.Or).Value("or");

        public static readonly TokenListParser<TokenKind, string> Eq =
            Token.EqualTo(TokenKind.EqualEqual).Value("==");

        public static readonly TokenListParser<TokenKind, string> Ne =
            Token.EqualTo(TokenKind.BangEqual).Value("!=");

        public static readonly TokenListParser<TokenKind, string> Lt =
            Token.EqualTo(TokenKind.Less).Value("<");

        public static readonly TokenListParser<TokenKind, string> Gt =
            Token.EqualTo(TokenKind.Less).Value(">");

        public static readonly TokenListParser<TokenKind, string> Add =
            Token.EqualTo(TokenKind.Plus).Value("+");

        public static readonly TokenListParser<TokenKind, string> Subtract =
            Token.EqualTo(TokenKind.Minus).Value("-");

        public static readonly TokenListParser<TokenKind, string> Multiply =
            Token.EqualTo(TokenKind.Star).Value("*");

        public static readonly TokenListParser<TokenKind, string> Divide =
            Token.EqualTo(TokenKind.Slash).Value("/");

        public static readonly TokenListParser<TokenKind, Expr> String =
            Token.EqualTo(TokenKind.String)
                .Select(x => x.ToStringValue().Trim('"'))
                .Select(x => (Expr)new Expr.Literal(x));

        public static readonly TokenListParser<TokenKind, Expr> Number =
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
                
        public static readonly TokenListParser<TokenKind, Expr> Literal =
            String
                .Or(Number)
                .Or(True)
                .Or(False)
                .Or(Nil);

        public static readonly TokenListParser<TokenKind, Expr> Factor =
            (from lparen in Token.EqualTo(TokenKind.LeftParen)
             from expr in Parse.Ref(() => Expr)
             from rparen in Token.EqualTo(TokenKind.RightParen)
             select (Expr)new Expr.Grouping(expr)).Or(Literal);

        public static readonly TokenListParser<TokenKind, Expr> Operand =
            (from op in Negate.Or(Not)
             from factor in Factor
             select MakeUnary(op, factor)).Or(Factor).Named("expression");

        public static readonly TokenListParser<TokenKind, Expr> Term =
            Parse.Chain(Multiply.Or(Divide), Operand, MakeBinary);

        public static readonly TokenListParser<TokenKind, Expr> Comparand =
            Parse.Chain(Add.Or(Subtract), Term, MakeBinary);

        public static readonly TokenListParser<TokenKind, Expr> Comparison =
            Parse.Chain(Lt.Or(Ne).Or(Gt).Or(Eq), Comparand, MakeBinary);

        public static readonly TokenListParser<TokenKind, Expr> Conjunction =
            Parse.Chain(And, Comparison, MakeBinary);

        public static readonly TokenListParser<TokenKind, Expr> Disjunction =
            Parse.Chain(Or, Conjunction, MakeBinary);

        public static readonly TokenListParser<TokenKind, Expr> Expr = Disjunction;

        private static Expr MakeBinary(string op, Expr left, Expr right) =>
            new Expr.Binary(left, op, right);

        private static Expr MakeUnary(string op, Expr right) =>
            new Expr.Unary(op, right);
    }
}