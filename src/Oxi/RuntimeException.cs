namespace Oxi
{
    using System;
    using Optional;
    using Superpower.Model;

    public class RuntimeException : Exception
    {
        public RuntimeException()
        {
        }

        public RuntimeException(string message)
            : base(message)
        {
        }

        public RuntimeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public RuntimeException(string message, Position position)
            : this(message)
        {
            this.Position = Option.Some(position);
        }

        public Option<Position> Position { get; } = Option.None<Position>();
    }
}