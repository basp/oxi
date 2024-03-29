﻿namespace Oxi;

using Superpower.Model;

public abstract class Expr
{
    public interface IVisitor<T>
    {
        T VisitBinary(Binary expr);

        T VisitGrouping(Grouping expr);

        T VisitIdentifier(Identifier expr);

        T VisitLiteral(Literal expr);

        T VisitList(List expr);

        T VisitUnary(Unary expr);

        T VisitTryExpr(Expr.Try expr);

        T VisitFunctionCall(FunctionCall expr);

        T VisitVerbCall(VerbCall expr);

        T VisitRange(Range expr);

        T VisitProperty(Property expr);
    }

    public abstract Token<TokenKind> Token { get; }

    public abstract T Accept<T>(IVisitor<T> visitor);

    public class Range : Expr
    {
        public Range(Token<TokenKind> tok, Expr from, Expr to)
        {
            this.Token = tok;
            this.From = from;
            this.To = to;
        }

        public override Token<TokenKind> Token { get; }

        public Expr From { get; }

        public Expr To { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitRange(this);
    }

    public class Property : Expr
    {
        public Property(
            Token<TokenKind> tok,
            Expr obj,
            Expr name)
        {
            this.Token = tok;
            this.Object = obj;
            this.Name = name;
        }

        public override Token<TokenKind> Token { get; }

        public Expr Object { get; }

        public Expr Name { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitProperty(this);
    }

    public class Binary : Expr
    {
        public Binary(
            Token<TokenKind> tok,
            Expr left,
            string op,
            Expr right)
        {
            this.Token = tok;
            this.Left = left;
            this.Op = op;
            this.Right = right;
        }

        public Expr Left { get; }

        public string Op { get; }

        public Expr Right { get; }

        public override Token<TokenKind> Token { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitBinary(this);
    }

    public class Grouping : Expr
    {
        public Grouping(Token<TokenKind> tok, Expr expr)
        {
            this.Token = tok;
            this.Expression = expr;
        }

        public Expr Expression { get; }

        public override Token<TokenKind> Token { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitGrouping(this);
    }

    public class Try : Expr
    {
        public Try(Token<TokenKind> tok, Expr expr, Expr[] errors, Expr alt)
        {
            this.Token = tok;
            this.Expr = expr;
            this.Errors = errors;
            this.Alternative = alt;
        }

        public Expr Expr { get; }

        public Expr[] Errors { get; }

        public Expr Alternative { get; }

        public override Token<TokenKind> Token { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitTryExpr(this);
    }

    public class Identifier : Expr
    {
        public Identifier(Token<TokenKind> tok, string value)
        {
            this.Token = tok;
            this.Value = value;
        }

        public string Value { get; }

        public override Token<TokenKind> Token { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitIdentifier(this);
    }

    public class List : Expr
    {
        public List(Token<TokenKind> tok, Expr[] elements)
        {
            this.Token = tok;
            this.Elements = elements;
        }

        public override Token<TokenKind> Token { get; }

        public Expr[] Elements { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitList(this);
    }

    public class Literal : Expr
    {
        public Literal(Token<TokenKind> tok, IValue value)
        {
            this.Token = tok;
            this.Value = value;
        }

        public IValue Value { get; }

        public override Token<TokenKind> Token { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitLiteral(this);
    }

    public class Unary : Expr
    {
        public Unary(Token<TokenKind> tok, string op, Expr right)
        {
            this.Token = tok;
            this.Op = op;
            this.Right = right;
        }

        public string Op { get; }

        public Expr Right { get; }

        public override Token<TokenKind> Token { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitUnary(this);
    }

    public class VerbCall : Expr
    {
        public VerbCall(Token<TokenKind> tok, Expr obj, Expr verb, Expr[] args)
        {
            this.Token = tok;
            this.Object = obj;
            this.Verb = verb;
            this.Arguments = args;
        }

        public Expr Object { get; }

        public Expr Verb { get; }

        public Expr[] Arguments { get; }

        public override Token<TokenKind> Token { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitVerbCall(this);
    }

    public class FunctionCall : Expr
    {
        public FunctionCall(Token<TokenKind> tok, Expr fn, Expr[] args)
        {
            this.Token = tok;
            this.Function = fn;
            this.Arguments = args;
        }

        public Expr Function { get; }

        public Expr[] Arguments { get; }

        public override Token<TokenKind> Token { get; }

        public override T Accept<T>(IVisitor<T> visitor) =>
            visitor.VisitFunctionCall(this);
    }
}
