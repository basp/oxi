namespace Oxi;

public class Runtime : IRuntime
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
