namespace Oxi
{
    using System;

    public interface IKernel
    {
        IValue TypeOf(IValue[] args);

        IValue Valid(IValue[] args);
    }

    public class Kernel : IKernel
    {
        public IValue TypeOf(params IValue[] args)
        {
            return new Value.Integer((int)args[0].Kind);
        }

        public IValue Valid(params IValue[] args)
        {
            return Value.Boolean.True;
        }
    }
}