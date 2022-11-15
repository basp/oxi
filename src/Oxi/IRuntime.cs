namespace Oxi;

public interface IRuntime
{
    IValue TypeOf(IValue[] args);

    IValue Valid(IValue[] args);
}