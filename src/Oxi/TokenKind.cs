namespace Oxi;

using Superpower.Display;

public enum TokenKind
{
    None,
    Comment,

    // one character tokens
    [Token(Example = "`")]
    Acute,

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

    [Token(Category = "operator", Example = ";")]
    Semicolon,

    [Token(Example = "?")]
    Question,

    // one or two character tokens
    [Token(Category = "operator", Example = "&")]
    Ampersand,

    [Token(Category = "operator", Example = "&&")]
    AmpersandAmpersand,

    [Token(Category = "operator", Example = "!")]
    Bang,

    [Token(Category = "operator", Example = "!=")]
    BangEqual,

    [Token(Category = "operator", Example = "|")]
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
    [Token(Category = "literal")]
    Identifier,

    [Token(Category = "literal")]
    String,

    [Token(Category = "literal")]
    Integer,

    [Token(Category = "literal")]
    Float,

    [Token(Category = "literal")]
    Object,

    [Token(Category = "literal")]
    Error,

    // keywords
    [Token(Category = "keyword", Example = "any")]
    Any,

    [Token(Category = "keyword", Example = "break")]
    Break,

    [Token(Category = "keyword", Example = "continue")]
    Continue,

    [Token(Category = "keyword", Example = "else")]
    Else,

    [Token(Category = "keyword", Example = "elseif")]
    ElseIf,

    [Token(Category = "keyword", Example = "endfor")]
    EndFor,

    [Token(Category = "keyword", Example = "endfork")]
    EndFork,

    [Token(Category = "keyword", Example = "endif")]
    EndIf,

    [Token(Category = "keyword", Example = "endtry")]
    EndTry,

    [Token(Category = "keyword", Example = "endwhile")]
    EndWhile,

    [Token(Category = "keyword", Example = "except")]
    Except,

    [Token(Category = "keyword", Example = "false")]
    False,

    [Token(Category = "keyword", Example = "for")]
    For,

    [Token(Category = "keyword", Example = "fork")]
    Fork,

    [Token(Category = "keyword", Example = "if")]
    If,

    [Token(Category = "keyword", Example = "in")]
    In,

    [Token(Category = "keyword", Example = "return")]
    Return,

    [Token(Category = "keyword", Example = "true")]
    True,

    [Token(Category = "keyword", Example = "try")]
    Try,

    [Token(Category = "keyword", Example = "while")]
    While,
}
