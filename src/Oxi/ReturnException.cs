namespace Oxi
{
    using System;

    internal class ReturnException : Exception
    {
        public ReturnException(IValue value)
        {
            this.Value = value;
        }

        public IValue Value { get; }
    }
}