namespace Oxi;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Superpower.Model;

public class Interpreter : Expr.IVisitor<IValue>, Stmt.IVisitor<IValue>
{
    private readonly IRuntime runtime = new Runtime();

    private readonly IDictionary<string, Func<IValue[], IValue>> bi;

    private readonly Stack<Environment> env =
        new Stack<Environment>(new[]
        {
            // global scope (might need to move this later)
            new Environment()
            {
                ["INT"] = new Value.Integer(ValueKind.Integer),
                ["FLOAT"] = new Value.Integer(ValueKind.Float),
                ["STRING"] = new Value.Integer(ValueKind.String),
                ["LIST"] = new Value.Integer(ValueKind.List),
                ["ERR"] = new Value.Integer(ValueKind.Error),
                ["E_NONE"] = new Value.Integer(0),
                ["E_PROPNF"] = new Value.Integer(1),
            },
        });

    private readonly IDatabase db;

    public Interpreter(IDatabase db)
    {
        this.db = db;

        this.bi = new Dictionary<string, Func<IValue[], IValue>>()
        {
            ["typeof"] = this.runtime.TypeOf,
            ["valid"] = this.runtime.Valid,
            ["pickle"] = TestSerialize,
            ["unpickle"] = TestDeserialize,
            ["maxobject"] = _ =>
            {
                var x = this.db.GetMaxObject();
                return new Value.Object(x);
            },
            ["create"] = _ =>
            {
                var x = this.db.Create();
                return new Value.Integer(x);
            },
            ["recycle"] = args =>
            {
                if (args.Length < 1)
                {
                    return Value.Error.INVARG;
                }

                if (args[0] is not Value.Object id)
                {
                    return Value.Error.INVARG;
                }

                var x = this.db.Recycle(id.Value);
                return new Value.Integer(x);
            },
            ["add_property"] = args =>
            {
                if (args.Length < 2)
                {
                    return Value.Error.INVARG;
                }

                if (args[0] is not Value.Object id)
                {
                    return Value.Error.INVARG;
                }

                if (args[1] is not Value.String name)
                {
                    return Value.Error.INVARG;
                }

                this.db.AddProperty(id.Value, name.Value);
                return new Value.Integer(1);
            },
            ["properties"] = args =>
            {
                if (args.Length < 1)
                {
                    return Value.Error.INVARG;
                }

                if (args[0] is not Value.Object id)
                {
                    return Value.Error.INVARG;
                }

                var names = this.db.GetProperties(id.Value);
                var xs = names
                    .Select(x => new Value.String(x))
                    .ToArray();

                return new Value.List(xs);
            },
        };
    }

    public IValue Exec(Stmt stmt)
    {
        var val = Interpret(() => stmt.Accept(this));
        return ThrowIfError(val, stmt.Token.Position);
    }

    public IValue Eval(Expr expr)
    {
        var val = Interpret(() => expr.Accept(this));
        return ThrowIfError(val, expr.Token.Position);
    }

    public IValue VisitExprStmt(Stmt.ExprStmt stmt) =>
        ThrowIfError(
            this.Eval(stmt.Expression),
            stmt.Token.Position);

    public IValue VisitBlock(Stmt.Block block)
    {
        IValue result = null;
        foreach (var stmt in block.Body)
        {
            result = ThrowIfError(stmt.Accept(this), stmt.Token.Position);
        }

        return result;
    }

    public IValue VisitReturn(Stmt.Return stmt)
    {
        var value = ThrowIfError(
            this.Eval(stmt.Expression),
            stmt.Token.Position);

        throw new ReturnException(value);
    }

    public IValue VisitIfStmt(Stmt.If stmt)
    {
        var arms = stmt.Conditions.Zip(
            stmt.Consequences,
            (cond, cons) => new { cond, cons });

        foreach (var arm in arms)
        {
            var cond = arm.cond.Accept(this);
            if (cond.IsTruthy)
            {
                return arm.cons.Accept(this);
            }
        }

        if (stmt.Alternative != null)
        {
            return stmt.Alternative.Accept(this);
        }

        return Value.Boolean.False;
    }

    public IValue VisitTryStmt(Stmt.Try stmt)
    {
        throw new NotImplementedException();
    }

    public IValue VisitForStmt(Stmt.For stmt)
    {
        var cond = ThrowIfError(
            stmt.Condition.Accept(this),
            stmt.Token.Position);

        var id = stmt.Id switch
        {
            Expr.Identifier x => x.Value,
            _ => throw new InvalidOperationException(),
        };

        IValue result = Value.Boolean.False;
        var scope = this.env.Peek();
        if (cond is IAggregate agg)
        {
            foreach (var val in agg)
            {
                scope[id] = val;
                result = stmt.Body.Accept(this);
            }
        }

        return result;
    }

    public IValue VisitBinary(Expr.Binary expr)
    {
        // special treatment for assignment
        if (expr.Op == "=")
        {
            return this.Assign(expr);
        }

        var left = ThrowIfError(
            this.Eval(expr.Left),
            expr.Left.Token.Position);

        if (expr.Op == "&&" && !left.IsTruthy)
        {
            // short-circuit `and` operator
            // don't evaluale right-hand side when left is false
            return Value.Boolean.False;
        }

        var right = ThrowIfError(
            this.Eval(expr.Right),
            expr.Right.Token.Position);

        IValue result = expr.Op switch
        {
            "==" => AreEqual(left, right),
            "!=" => AreNotEqual(left, right),
            "<" => LessThan(left, right),
            ">" => GreaterThan(left, right),
            "<=" => LessThanOrEqual(left, right),
            ">=" => GreaterThanOrEqual(left, right),
            "-" => Subtract(left, right),
            "+" => Add(left, right),
            "/" => Divide(left, right),
            "%" => Remainder(left, right),
            "*" => Multiply(left, right),
            "&&" => And(left, right),
            "||" => Or(left, right),
            "^" => Xor(left, right),
            _ => throw new RuntimeException(
                $"invalid binary operation `{expr.Op}`",
                expr.Token.Position),
        };

        return ThrowIfError(result, expr.Token.Position);
    }

    public IValue VisitGrouping(Expr.Grouping expr) =>
        this.Eval(expr.Expression);

    public IValue VisitUnary(Expr.Unary expr)
    {
        var right = ThrowIfError(
            this.Eval(expr.Right),
            expr.Right.Token.Position);

        IValue result = expr.Op switch
        {
            "!" => Value.Boolean.Get(!right.IsTruthy),
            "-" => right.Negate(),
            _ => throw new RuntimeException(
                $"invalid unary operation `{expr.Op}`",
                expr.Token.Position),
        };

        return ThrowIfError(result, expr.Token.Position);
    }

    public IValue VisitRange(Expr.Range expr)
    {
        IValue CreateObjectRange(int from, int to)
        {
            if (to <= from)
            {
                return Value.List.Empty;
            }

            var count = to - from + 1;
            var xs = Enumerable
                .Range(from, count)
                .Select(x => new Value.Object(x))
                .Where(x => true) // TODO: filter valid(obj)
                .Cast<IValue>()
                .ToList();

            return new Value.List(xs);
        }

        IValue CreateIntegerRange(int from, int to)
        {
            if (to <= from)
            {
                return Value.List.Empty;
            }

            var count = to - from + 1;
            var xs = Enumerable
                .Range(from, count)
                .Select(x => new Value.Integer(x))
                .Cast<IValue>()
                .ToList();

            return new Value.List(xs);
        }

        var from = expr.From.Accept(this);
        var to = expr.To.Accept(this);
        return (from, to) switch
        {
            (Value.Integer x, Value.Integer y) =>
                CreateIntegerRange(x.Value, y.Value),
            (Value.Object x, Value.Object y) =>
                CreateObjectRange(x.Value, y.Value),
            (Value.Error x, _) => x,
            (_, Value.Error y) => y,
            _ => Value.Error.INVARG,
        };
    }

    public IValue VisitList(Expr.List expr)
    {
        var xs = expr.Elements.Select(x => x.Accept(this)).ToArray();
        return new Value.List(xs);
    }

    public IValue VisitIdentifier(Expr.Identifier expr)
    {
        var scope = this.env.Peek();
        if (scope.TryGetValue(expr.Value, out var value))
        {
            return value;
        }

        return Value.Error.VARNF;
    }

    public IValue VisitLiteral(Expr.Literal expr) => expr.Value;

    public IValue VisitTryExpr(Expr.Try expr)
    {
        throw new NotImplementedException();
    }

    public IValue VisitFunctionCall(Expr.FunctionCall expr)
    {
        var id = (Expr.Identifier)expr.Function;
        if (this.bi.TryGetValue(id.Value, out var fn))
        {
            var args = expr.Arguments
                .Select(x => x.Accept(this))
                .ToArray();

            return fn(args);
        }

        // invalid built-in function name
        throw new NotImplementedException(id.Value);
    }

    public IValue VisitVerbCall(Expr.VerbCall expr)
    {
        throw new NotImplementedException();
    }

    public IValue VisitProperty(Expr.Property expr)
    {
        throw new NotImplementedException();
    }

    private static IValue Interpret(Func<IValue> visit)
    {
        try
        {
            return visit();
        }
        catch (ReturnException ex)
        {
            return ex.Value;
        }
    }

    private static IValue ThrowIfError(IValue value, Position position)
    {
        if (value is Value.Error err)
        {
            throw new RuntimeException(err.Message, position);
        }

        return value;
    }

    private static IValue Negate(IValue value) => value.Negate();

    private static IValue And(IValue left, IValue right) =>
        Value.Boolean.Get(left.IsTruthy && right.IsTruthy);

    private static IValue Or(IValue left, IValue right) =>
        Value.Boolean.Get(left.IsTruthy || right.IsTruthy);

    private static IValue Xor(IValue left, IValue right) =>
        Value.Boolean.Get(left.IsTruthy ^ right.IsTruthy);

    private static IValue Add(IValue left, IValue right) =>
        (left, right) switch
        {
            (IFloatable x, IFloatable y) => x.Add(y),
            (Value.String x, Value.String y) =>
                new Value.String(x.Value + y.Value),
            (Value.String x, IValue y) =>
                new Value.String(x.Value + y.ToString()),
            (IValue x, Value.String y) =>
                new Value.String(x.ToString() + y.Value),
            _ => Value.Error.TYPE,
        };

    private static IValue Subtract(IValue left, IValue right) =>
        (left, right) switch
        {
            (IFloatable x, IFloatable y) => x.Sub(y),
            _ => Value.Error.TYPE,
        };

    private static IValue Multiply(IValue left, IValue right) =>
        (left, right) switch
        {
            (IFloatable x, IFloatable y) => x.Mul(y),
            _ => Value.Error.TYPE,
        };

    private static IValue Divide(IValue left, IValue right) =>
        (left, right) switch
        {
            (IFloatable x, IFloatable y) => x.Divide(y),
            _ => Value.Error.TYPE,
        };

    private static IValue Remainder(IValue left, IValue right) =>
        (left, right) switch
        {
            (IFloatable x, IFloatable y) => x.Rem(y),
            _ => Value.Error.TYPE,
        };

    private static IValue AreEqual(object a, object b)
    {
        if (a == null && b == null)
        {
            return Value.Boolean.True;
        }

        if (a == null)
        {
            return Value.Boolean.False;
        }

        return Value.Boolean.Get(a.Equals(b));
    }

    private static IValue AreNotEqual(object a, object b)
    {
        if (a == null && b == null)
        {
            return Value.Boolean.False;
        }

        if (a == null)
        {
            return Value.Boolean.True;
        }

        return Value.Boolean.Get(!a.Equals(b));
    }

    private static IValue Compare(IValue left, IValue right) =>
        (left, right) switch
        {
            (IOrdinal x, IOrdinal y) => new Value.Integer(x.CompareTo(y)),
            _ => Value.Error.TYPE,
        };

    private static IValue LessThan(IValue left, IValue right) =>
        (left, right) switch
        {
            (IOrdinal x, IOrdinal y) =>
                Value.Boolean.Get(x.OrdinalValue < y.OrdinalValue),
            _ => Value.Error.TYPE,
        };

    private static IValue LessThanOrEqual(IValue left, IValue right) =>
        (left, right) switch
        {
            (IOrdinal x, IOrdinal y) =>
                Value.Boolean.Get(x.OrdinalValue <= y.OrdinalValue),
            _ => Value.Error.TYPE,
        };

    private static IValue GreaterThan(IValue left, IValue right) =>
        (left, right) switch
        {
            (IOrdinal x, IOrdinal y) =>
                Value.Boolean.Get(x.OrdinalValue > y.OrdinalValue),
            _ => Value.Error.TYPE,
        };

    private static IValue GreaterThanOrEqual(IValue left, IValue right) =>
        (left, right) switch
        {
            (IOrdinal x, IOrdinal y) =>
                Value.Boolean.Get(x.OrdinalValue >= y.OrdinalValue),
            _ => Value.Error.TYPE,
        };

    private static IValue TestSerialize(IValue[] args)
    {
        if (args.Length < 1)
        {
            return Value.List.Empty;
        }

        using var stream = new MemoryStream();
        using var writer = new ValueWriter(stream);
        using var reader = new BinaryReader(stream);

        args[0].Accept(writer);

        // Support up to max int for now, might wanna throw otherwise.
        var numberOfBytes = (int)stream.Position;
        stream.Position = 0;
        var xs = Enumerable
            .Range(0, numberOfBytes)
            .Select(_ => reader.ReadByte())
            .Select(x => new Value.Integer(x))
            .ToArray();

        return new Value.List(xs);
    }

    private static IValue TestDeserialize(IValue[] args)
    {
        if (args.Length < 1)
        {
            return Value.Boolean.False;
        }

        if (args[0] is not Value.List)
        {
            return Value.Error.TYPE;
        }

        var list = (Value.List)args[0];
        var buffer = list.Value
            .Cast<Value.Integer>()
            .Select(x => (byte)x.Value)
            .ToArray();

        using var stream = new MemoryStream(buffer);
        using var reader = new ValueReader(stream);
        return reader.Read();
    }

    private IValue Assign(Expr.Binary expr)
    {
        var scope = this.env.Peek();
        var id = expr.Left switch
        {
            Expr.Identifier x => x.Value,
            _ => throw new InvalidOperationException(),
        };

        var value = ThrowIfError(
            expr.Right.Accept(this),
            expr.Right.Token.Position);

        scope[id] = value;
        return value;
    }
}
