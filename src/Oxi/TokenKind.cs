namespace Oxi
{
    public enum TokenKind
    {
        None,
        Comment,

        // one character tokens
        LeftParen,
        RightParen,
        LeftBrace,
        RightBrace,
        LeftBracket,
        RightBracket,
        Dollar,
        Pound,
        Comma,
        Dot,
        Plus,
        Minus,
        Star,
        Slash,
        Percent,
        Colon,
        Semicolon,
        Question,

        // one or two character tokens
        Ampersand,
        AmpersandAmpersand,
        Bang,
        BangEqual,
        Bar,
        BarBar,
        Equal,
        EqualEqual,
        EqualGreater,
        Greater,
        GreaterEqual,
        Less,
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