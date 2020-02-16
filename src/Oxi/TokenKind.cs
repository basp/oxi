namespace Oxi
{
    public enum TokenKind
    {
        None,
        Illegal,
        EOF,

        // one character tokens
        LeftParen,
        RightParen,
        LeftBrace,
        RightBrace,
        LeftBrack,
        RightBrack,
        Comma,
        Dot,
        Minus,
        Plus,
        Colon,
        Semicolon,
        Slash,
        Star,
        Question,

        // one or two character tokens
        Bang,
        BangEqual,
        Equal,
        EqualEqual,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,

        // literals
        Identifier,
        String,
        Number,

        // keywords
        Function,
        Let,
        And,
        Or,
        True,
        False,
    }
}