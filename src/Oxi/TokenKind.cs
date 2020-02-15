namespace Oxi
{
    public enum TokenKind
    {
        None,
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
        Int,
        Float,

        // keywords
        And,
        Class,
        Else,
        False,
        Fun,
        For,
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