namespace Oxi;

using System;
using System.IO;
using System.Linq;

public class ValueReader : IValueReader, IDisposable
{
    private readonly BinaryReader reader;

    private bool disposed = false;

    public ValueReader(Stream stream)
    {
        this.reader = new BinaryReader(stream);
    }

    public IValue Read()
    {
        var kind = (ValueKind)this.reader.ReadInt32();
        switch (kind)
        {
            case ValueKind.Boolean:
                return this.ReadBoolean();
            case ValueKind.Integer:
                return this.ReadInteger();
            case ValueKind.Character:
                return this.ReadCharacter();
            case ValueKind.Float:
                return this.ReadFloat();
            case ValueKind.Object:
                return this.ReadObject();
            case ValueKind.List:
                return this.ReadList();
        }

        throw new NotImplementedException();
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
            this.reader.Dispose();
        }

        this.disposed = true;
    }

    private IValue ReadBoolean()
    {
        var value = this.reader.ReadBoolean();
        return value ? Value.Boolean.True : Value.Boolean.False;
    }

    private IValue ReadInteger()
    {
        var value = this.reader.ReadInt32();
        return new Value.Integer(value);
    }

    private IValue ReadCharacter()
    {
        var value = this.reader.ReadChar();
        return new Value.Character(value);
    }

    private IValue ReadFloat()
    {
        var value = this.reader.ReadDouble();
        return new Value.Float(value);
    }

    private IValue ReadString()
    {
        var value = this.reader.ReadString();
        return new Value.String(value);
    }

    private IValue ReadObject()
    {
        var value = this.reader.ReadInt32();
        return new Value.Object(value);
    }

    private IValue ReadList()
    {
        var count = this.reader.ReadInt32();
        var xs = Enumerable
            .Range(0, count)
            .Select(_ => this.Read())
            .ToList();

        return new Value.List(xs);
    }
}