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

        private static readonly IDictionary<string, TokenKind> Keywords =
            new Dictionary<string, TokenKind>
            {
                ["ANY"] = TokenKind.Any,
                ["break"] = TokenKind.Break,
                ["continue"] = TokenKind.Continue,
                ["else"] = TokenKind.Else,
                ["elseif"] = TokenKind.ElseIf,
                ["endfor"] = TokenKind.EndFor,
                ["endfork"] = TokenKind.EndFork,
                ["endif"] = TokenKind.EndIf,
                ["endtry"] = TokenKind.EndTry,
                ["endwhile"] = TokenKind.EndWhile,
                ["false"] = TokenKind.False,
                ["for"] = TokenKind.For,
                ["fork"] = TokenKind.Fork,
                ["if"] = TokenKind.If,
                ["in"] = TokenKind.In,
                ["return"] = TokenKind.Return,
                ["true"] = TokenKind.True,
                ["try"] = TokenKind.Try,
                ["while"] = TokenKind.While,
            };

        static Scanner()
        {
            SimpleOps['+'] = TokenKind.Plus;
            SimpleOps['-'] = TokenKind.Minus;
            SimpleOps['*'] = TokenKind.Star;
            SimpleOps['/'] = TokenKind.Slash;
            SimpleOps['%'] = TokenKind.Percent;
            SimpleOps['<'] = TokenKind.Less;
            SimpleOps['>'] = TokenKind.Greater;
            SimpleOps['='] = TokenKind.Equal;
            SimpleOps[','] = TokenKind.Comma;
            SimpleOps['.'] = TokenKind.Dot;
            SimpleOps[':'] = TokenKind.Colon;
            SimpleOps[';'] = TokenKind.Semicolon;
            SimpleOps['('] = TokenKind.LeftParen;
            SimpleOps[')'] = TokenKind.RightParen;
            SimpleOps['['] = TokenKind.LeftBracket;
            SimpleOps[']'] = TokenKind.RightBracket;
            SimpleOps['{'] = TokenKind.LeftBrace;
            SimpleOps['}'] = TokenKind.RightBrace;
            SimpleOps['!'] = TokenKind.Bang;
            SimpleOps['?'] = TokenKind.Question;
            SimpleOps['$'] = TokenKind.Dollar;
            SimpleOps['#'] = TokenKind.Pound;
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
                if (IsIdentifierStartCharacter(next.Value))
                {
                    var id = Identifier.CStyle(next.Location);
                    next = id.Remainder.ConsumeChar();
                    if (Keywords.TryGetValue(id.Value.ToStringValue(), out var kind))
                    {
                        yield return Result.Value(
                            kind,
                            id.Location,
                            id.Remainder);
                    }
                    else
                    {
                        yield return Result.Value(
                            TokenKind.Identifier,
                            id.Location,
                            id.Remainder);
                    }
                }
                else if (char.IsDigit(next.Value))
                {
                    var num = Numerics.Decimal(next.Location);
                    var kind = int.TryParse(num.Value.ToStringValue(), out var _)
                        ? TokenKind.Integer
                        : TokenKind.Float;

                    next = num.Remainder.ConsumeChar();
                    yield return Result.Value(
                        kind,
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
                    Result<TextSpan> comment;

                    comment = Comment.CPlusPlusStyle(next.Location);
                    if (comment.HasValue)
                    {
                        yield return Result.Value(
                            TokenKind.Comment,
                            comment.Location,
                            comment.Remainder);

                        next = comment.Remainder.ConsumeChar();
                        goto done;
                    }

                    comment = Comment.CStyle(next.Location);
                    if (comment.HasValue)
                    {
                        yield return Result.Value(
                            TokenKind.Comment,
                            comment.Location,
                            comment.Remainder);

                        next = comment.Remainder.ConsumeChar();
                        goto done;
                    }

                    yield return Result.Value(
                        SimpleOps[next.Value],
                        next.Location,
                        next.Remainder);

                    next = next.Remainder.ConsumeChar();
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
                        goto done;
                    }

                    if (next.Value < SimpleOps.Length && SimpleOps[next.Value] != TokenKind.None)
                    {
                        yield return Result.Value(
                            SimpleOps[next.Value],
                            next.Location,
                            next.Remainder);

                        next = next.Remainder.ConsumeChar();
                        goto done;
                    }

                    yield return Result.Empty<TokenKind>(next.Location);
                    next = next.Remainder.ConsumeChar();
                }

            done:
                next = SkipWhiteSpace(next.Location);
            }
            while (next.HasValue);
        }

        private static bool IsIdentifierStartCharacter(char c) =>
            c == '_' || char.IsLetter(c);
    }
}