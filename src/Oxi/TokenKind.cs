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
        And,
        Class,
        Else,
        False,
        For,
        Fun,
        If,
        Nil,
        Or,
        Print,
        Return,
        Super,
        This,
        True,
        Var,
        While,
    }
}