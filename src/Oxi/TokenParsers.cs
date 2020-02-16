namespace Oxi
{
    using Superpower;
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

        private static readonly TokenListParser<TokenKind, Operator> Negate =
            Token
                .EqualTo(TokenKind.Minus)
                .Value(Operator.Neg);

        private static readonly TokenListParser<TokenKind, Operator> Not =
            Token
                .EqualTo(TokenKind.Bang)
                .Value(Operator.Not);

        private static readonly TokenListParser<TokenKind, Operator> And =
            Token
                .EqualTo(TokenKind.And)
                .Value(Operator.And);

        private static readonly TokenListParser<TokenKind, Operator> Or =
            Token
                .EqualTo(TokenKind.Or)
                .Value(Operator.Or);

        private static readonly TokenListParser<TokenKind, Operator> Eq =
            Token
                .EqualTo(TokenKind.EqualEqual)
                .Value(Operator.Eq);

        private static readonly TokenListParser<TokenKind, Operator> Ne =
            Token
                .EqualTo(TokenKind.BangEqual)
                .Value(Operator.Ne);

        private static readonly TokenListParser<TokenKind, Operator> Lt =
            Token.EqualTo(TokenKind.Less).Value(Operator.Lt);

        private static readonly TokenListParser<TokenKind, Operator> Gt =
            Token.EqualTo(TokenKind.Less).Value(Operator.Gt);

        private static readonly TokenListParser<TokenKind, Operator> Ge =
            Token.EqualTo(TokenKind.GreaterEqual).Value(Operator.Ge);

        private static readonly TokenListParser<TokenKind, Operator> Le =
            Token.EqualTo(TokenKind.LessEqual).Value(Operator.Le);

        private static readonly TokenListParser<TokenKind, Operator> Add =
            Token.EqualTo(TokenKind.Plus).Value(Operator.Add);

        private static readonly TokenListParser<TokenKind, Operator> Subtract =
            Token.EqualTo(TokenKind.Minus).Value(Operator.Sub);

        private static readonly TokenListParser<TokenKind, Operator> Multiply =
            Token.EqualTo(TokenKind.Star).Value(Operator.Mul);

        private static readonly TokenListParser<TokenKind, Operator> Divide =
            Token.EqualTo(TokenKind.Slash).Value(Operator.Div);

        private static readonly TokenListParser<TokenKind, Expr> String =
            Token.EqualTo(TokenKind.String)
                .Select(x => new { tok = x, val = x.ToStringValue().Trim('"') })
                .Select(x => (Expr)new Expr.Literal(x.tok, x.val));

        private static readonly TokenListParser<TokenKind, Expr> Number =
            Token.EqualTo(TokenKind.Number)
                .Select(x =>
                {
                    var str = x.ToStringValue();
                    if (int.TryParse(str, out var i))
                    {
                        return (Expr)new Expr.Literal(x, i);
                    }

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
            (from op in Negate.Or(Not)
             from factor in Factor
             select MakeUnary(op, factor)).Or(Factor).Named("expression");

        private static readonly TokenListParser<TokenKind, Expr> Term =
            Parse.Chain(Multiply.Or(Divide), Operand, MakeBinary);

        private static readonly TokenListParser<TokenKind, Expr> Comparand =
            Parse.Chain(Add.Or(Subtract), Term, MakeBinary);

        private static readonly TokenListParser<TokenKind, Expr> Comparison =
            Parse.Chain(Lt.Or(Ne).Or(Gt).Or(Eq), Comparand, MakeBinary);

        private static readonly TokenListParser<TokenKind, Expr> Conjunction =
            Parse.Chain(And, Comparison, MakeBinary);

        private static readonly TokenListParser<TokenKind, Expr> Disjunction =
            Parse.Chain(Or, Conjunction, MakeBinary);

        private static Expr MakeBinary(Operator op, Expr left, Expr right) =>
            new Expr.Binary(left, op.Name, right);

        private static Expr MakeUnary(Operator op, Expr right) =>
            new Expr.Unary(op.Name, right);
    }
}