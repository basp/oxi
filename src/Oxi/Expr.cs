namespace Oxi
{
    using System;

    public abstract class Expr
    {
        public interface IVisitor<T>
        {
        }

        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}
