namespace Oxi
{
    using System.Collections.Generic;
    using Superpower;
    using Superpower.Model;
    using Superpower.Parsers;

    public class Scanner : Tokenizer<TokenKind>
    {
        private static readonly TokenKind[] SimpleOps =
            new TokenKind[128];

        static Scanner()
        {
            SimpleOps['+'] = TokenKind.Plus;
            SimpleOps['-'] = TokenKind.Minus;
            SimpleOps['*'] = TokenKind.Star;
            SimpleOps['/'] = TokenKind.Slash;
            SimpleOps['<'] = TokenKind.Less;
            SimpleOps['>'] = TokenKind.Greater;
            SimpleOps['='] = TokenKind.Equal;
            SimpleOps[','] = TokenKind.Comma;
            SimpleOps['.'] = TokenKind.Dot;
            SimpleOps[':'] = TokenKind.Colon;
            SimpleOps[';'] = TokenKind.Semicolon;
            SimpleOps['('] = TokenKind.LeftParen;
            SimpleOps[')'] = TokenKind.RightParen;
            SimpleOps['['] = TokenKind.LeftBrack;
            SimpleOps[']'] = TokenKind.RightBrack;
            SimpleOps['{'] = TokenKind.LeftBrace;
            SimpleOps['}'] = TokenKind.RightBrace;
            SimpleOps['!'] = TokenKind.Bang;
            SimpleOps['?'] = TokenKind.Question;
        }

        protected override IEnumerable<Result<TokenKind>> Tokenize(
            TextSpan span,
            TokenizationState<TokenKind> state)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
            {
                yield break;
            }

            do
            {
                if (char.IsLetter(next.Value) || next.Value == '_')
                {
                    var id = Identifier.CStyle(next.Location);
                    next = id.Remainder.ConsumeChar();
                    yield return Result.Value(
                        TokenKind.Identifier,
                        id.Location,
                        id.Remainder);
                }
                else if (char.IsDigit(next.Value))
                {
                    var num = Numerics.Decimal(next.Location);
                    next = num.Remainder.ConsumeChar();
                    yield return Result.Value(
                        TokenKind.Number,
                        num.Location,
                        num.Remainder);
                }
                else if (next.Value == '"')
                {
                    var str = QuotedString.CStyle(next.Location);
                    if (!str.HasValue)
                    {
                        yield return Result.CastEmpty<string, TokenKind>(str);
                    }

                    next = str.Remainder.ConsumeChar();
                    yield return Result.Value(
                        TokenKind.String,
                        str.Location,
                        str.Remainder);
                }
                else if (next.Value == '/')
                {
                    var comment = Comment.CPlusPlusStyle(next.Location);
                    if (comment.HasValue)
                    {
                        yield return Result.Value(
                            TokenKind.None,
                            comment.Location,
                            comment.Remainder);

                        next = comment.Remainder.ConsumeChar();
                    }
                    else
                    {
                        yield return Result.Value(
                            SimpleOps[next.Value],
                            next.Location,
                            next.Remainder);

                        next = next.Remainder.ConsumeChar();
                    }
                }
                else if (next.Value == '#')
                {
                    var comment = Comment.ShellStyle(next.Location);
                    next = comment.Remainder.ConsumeChar();
                    yield return Result.Value(
                        TokenKind.None,
                        comment.Location,
                        comment.Remainder);
                }
                else
                {
                    var compoundOp = TextParsers.CompoundOperator(next.Location);
                    if (compoundOp.HasValue)
                    {
                        yield return Result.Value(
                            compoundOp.Value,
                            compoundOp.Location,
                            compoundOp.Remainder);

                        next = compoundOp.Remainder.ConsumeChar();
                    }
                    else if (next.Value < SimpleOps.Length && SimpleOps[next.Value] != TokenKind.None)
                    {
                        yield return Result.Value(
                            SimpleOps[next.Value],
                            next.Location,
                            next.Remainder);

                        next = next.Remainder.ConsumeChar();
                    }
                    else
                    {
                        yield return Result.Empty<TokenKind>(next.Location);
                        next = next.Remainder.ConsumeChar();
                    }
                }

                next = SkipWhiteSpace(next.Location);
            }
            while (next.HasValue);
        }
    }
}