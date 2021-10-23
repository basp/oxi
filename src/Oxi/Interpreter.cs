namespace Oxi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Superpower.Model;

    public class Interpreter : Expr.IVisitor<IValue>, Stmt.IVisitor<IValue>
    {
        private readonly Stack<Environment> env =
            new Stack<Environment>(new[] { new Environment() });

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

            return Value.Boolean.Get(false);
        }

        public IValue VisitForStmt(Stmt.For stmt)
        {
            var id = ThrowIfError(
                stmt.Id.Accept(this),
                stmt.Id.Token.Position);

            var cond = ThrowIfError(
                stmt.Condition.Accept(this),
                stmt.Token.Position);

            string key = id switch
            {
                _ => throw new NotImplementedException(),
            };

            IValue result;
            var scope = this.env.Peek();
            if (cond is IAggregate agg)
            {
                foreach (var val in agg)
                {
                    scope[key] = val;
                    result = stmt.Body.Accept(this);
                }
            }

            return result;
        }

        public IValue VisitBinary(Expr.Binary expr)
        {
            // Assign operation needs special treatment
            if (expr.Op == "=")
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

            var left = ThrowIfError(
                this.Eval(expr.Left),
                expr.Left.Token.Position);

            // Short-circuit `and` operator when left side is false
            if (expr.Op == "&&" && !left.IsTruthy)
            {
                return Value.Boolean.Get(false);
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
                "-" => throw new NotImplementedException(),
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

                var count = to - from;
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

                var count = to - from;
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

        public IValue VisitFunctionCall(Expr.FunctionCall expr)
        {
            throw new NotImplementedException();
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
                return Value.Boolean.Get(true);
            }

            if (a == null)
            {
                return Value.Boolean.Get(false);
            }

            return Value.Boolean.Get(a.Equals(b));
        }

        private static IValue AreNotEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return Value.Boolean.Get(false);
            }

            if (a == null)
            {
                return Value.Boolean.Get(true);
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

        private void Scoped(Action<Environment> act)
        {
            var scope = new Environment(this.env.Peek());
            this.env.Push(scope);
            act(scope);
            this.env.Pop();
        }
    }
}