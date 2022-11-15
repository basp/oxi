namespace Oxi;

using System;
using System.IO;

public class ValueWriter : IValueWriter, IValue.IVisitor, IDisposable
{
    private readonly BinaryWriter writer;

    private bool disposed = false;

    public ValueWriter(Stream stream)
    {
        this.writer = new BinaryWriter(stream);
    }

    public void Write(IValue node) => node.Accept(this);

    public void VisitBoolean(Value.Boolean node)
    {
        this.WriteKind(ValueKind.Boolean);
        this.writer.Write(node.Value);
    }

    public void VisitCharacter(Value.Character node)
    {
        this.WriteKind(ValueKind.Character);
        this.writer.Write(node.Value);
    }

    public void VisitFloat(Value.Float node)
    {
        this.WriteKind(ValueKind.Float);
        this.writer.Write(node.Value);
    }

    public void VisitInteger(Value.Integer node)
    {
        this.WriteKind(ValueKind.Integer);
        this.writer.Write(node.Value);
    }

    public void VisitList(Value.List node)
    {
        this.WriteKind(ValueKind.List);
        var count = node.Value.Count;
        this.writer.Write(count);
        foreach (var elem in node.Value)
        {
            elem.Accept(this);
        }
    }

    public void VisitObject(Value.Object node)
    {
        this.WriteKind(ValueKind.Object);
        this.writer.Write(node.Value);
    }

    public void VisitString(Value.String node)
    {
        this.WriteKind(ValueKind.String);
        this.writer.Write(node.Value);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            this.writer.Dispose();
        }

        this.disposed = true;
    }

    private void WriteKind(ValueKind kind) => this.writer.Write((int)kind);
}
