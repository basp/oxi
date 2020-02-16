namespace Oxi
{
    using Superpower;
    using Superpower.Parsers;

    public static class TokenParsers
    {
        public static readonly TokenListParser<TokenKind, Expr> True =
            Token.EqualTo(TokenKind.True).Value((Expr)new Expr.Literal(true));

        public static readonly TokenListParser<TokenKind, Expr> False =
            Token.EqualTo(TokenKind.False).Value((Expr)new Expr.Literal(false));

        public static readonly TokenListParser<TokenKind, Expr> Nil =
            Token.EqualTo(TokenKind.Nil).Value((Expr)new Expr.Literal(null));

        public static readonly TokenListParser<TokenKind, Operator> Negate =
            Token.EqualTo(TokenKind.Minus).Value(Operator.Neg);

        public static readonly TokenListParser<TokenKind, Operator> Not =
            Token.EqualTo(TokenKind.BangEqual).Value(Operator.Not);

        public static readonly TokenListParser<TokenKind, Operator> And =
            Token.EqualTo(TokenKind.And).Value(Operator.And);

        public static readonly TokenListParser<TokenKind, Operator> Or =
            Token.EqualTo(TokenKind.Or).Value(Operator.Or);

        public static readonly TokenListParser<TokenKind, Operator> Eq =
            Token.EqualTo(TokenKind.EqualEqual).Value(Operator.Eq);

        public static readonly TokenListParser<TokenKind, Operator> Ne =
            Token.EqualTo(TokenKind.BangEqual).Value(Operator.Ne);

        public static readonly TokenListParser<TokenKind, Operator> Lt =
            Token.EqualTo(TokenKind.Less).Value(Operator.Lt);

        public static readonly TokenListParser<TokenKind, Operator> Gt =
            Token.EqualTo(TokenKind.Less).Value(Operator.Gt);

        public static readonly TokenListParser<TokenKind, Operator> Ge =
            Token.EqualTo(TokenKind.GreaterEqual).Value(Operator.Ge);

        public static readonly TokenListParser<TokenKind, Operator> Le =
            Token.EqualTo(TokenKind.LessEqual).Value(Operator.Le);

        public static readonly TokenListParser<TokenKind, Operator> Add =
            Token.EqualTo(TokenKind.Plus).Value(Operator.Add);

        public static readonly TokenListParser<TokenKind, Operator> Subtract =
            Token.EqualTo(TokenKind.Minus).Value(Operator.Sub);

        public static readonly TokenListParser<TokenKind, Operator> Multiply =
            Token.EqualTo(TokenKind.Star).Value(Operator.Mul);

        public static readonly TokenListParser<TokenKind, Operator> Divide =
            Token.EqualTo(TokenKind.Slash).Value(Operator.Div);

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

        private static Expr MakeBinary(Operator op, Expr left, Expr right) =>
            new Expr.Binary(left, op.Name, right);

        private static Expr MakeUnary(Operator op, Expr right) =>
            new Expr.Unary(op.Name, right);
    }
}