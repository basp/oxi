namespace Oxi
{
    using Superpower.Display;

    public enum TokenKind
    {
        None,
        Comment,

        // one character tokens
        [Token(Example = "^")]
        Caret,

        [Token(Example = "(")]
        LeftParen,

        [Token(Example = ")")]
        RightParen,

        [Token(Example = "{")]
        LeftBrace,

        [Token(Example = "}")]
        RightBrace,

        [Token(Example = "[")]
        LeftBracket,

        [Token(Example = "]")]
        RightBracket,

        [Token(Example = "$")]
        Dollar,

        [Token(Example = "#")]
        Pound,

        [Token(Example = ",")]
        Comma,

        [Token(Example = "+")]
        Plus,

        [Token(Example = "-")]
        Minus,

        [Token(Example = "*")]
        Star,

        [Token(Example = "/")]
        Slash,

        [Token(Example = "%")]
        Percent,

        [Token(Example = ":")]
        Colon,

        [Token(Example = ";")]
        Semicolon,

        [Token(Example = "?")]
        Question,

        // one or two character tokens
        [Token(Example = "&")]
        Ampersand,

        [Token(Category = "operator", Example = "&&")]
        AmpersandAmpersand,

        [Token(Category = "operator", Example = "!")]
        Bang,

        [Token(Category = "operator", Example = "!=")]
        BangEqual,

        [Token(Example = "|")]
        Bar,

        [Token(Category = "operator", Example = "||")]
        BarBar,

        [Token(Category = "operator", Example = ".")]
        Dot,

        [Token(Category = "operator", Example = "..")]
        DotDot,

        [Token(Category = "operator", Example = "=")]
        Equal,

        [Token(Category = "operator", Example = "==")]
        EqualEqual,

        [Token(Category = "operator", Example = "=>")]
        EqualGreater,

        [Token(Category = "operator", Example = ">")]
        Greater,

        [Token(Category = "operator", Example = ">=")]
        GreaterEqual,

        [Token(Category = "operator", Example = "<")]
        Less,

        [Token(Category = "operator", Example = "<=")]
        LessEqual,

        // literals
        Identifier,
        String,
        Integer,
        Float,
        Object,
        Error,

        // keywords
        Any,
        Break,
        Continue,
        Else,
        ElseIf,
        EndFor,
        EndFork,
        EndIf,
        EndTry,
        EndWhile,
        False,
        For,
        Fork,
        If,
        In,
        Return,
        True,
        Try,
        While,
    }
}