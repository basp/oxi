namespace Oxi.Tool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PowerArgs;
    using Superpower;
    using Superpower.Model;

    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    internal class Program
    {
        [HelpHook]
        public bool Help { get; set; }

        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Args.InvokeAction<Program>(args);
            }

            var interpreter = new Interpreter();
            var scanner = new Scanner();
            var printers = new Dictionary<Stmt.IVisitor<string>, bool>
            {
                [new AstPrinter()] = true,
                [new PrettyPrinter()] = false,
            };

            while (true)
            {
                Console.Write("> ");
                var src = Console.ReadLine();
                try
                {
                    var tokens = scanner
                        .Tokenize(src)
                        .Where(x => x.Kind != TokenKind.Comment)
                        .ToArray();

                    var list = new TokenList<TokenKind>(tokens);
                    var ast = Oxi.TokenParsers.Block.Parse(list);

                    foreach (var (printer, enabled) in printers)
                    {
                        if (enabled)
                        {
                            Console.WriteLine(ast.Accept(printer));
                        }
                    }

                    // var result = interpreter.Exec(ast);
                    // Console.WriteLine($"=> {Stringify(result)}");
                }
                catch (RuntimeException ex)
                {
                    Print(src, ex);
                }
                catch (ParseException ex)
                {
                    Print(src, ex);
                }
            }
        }

        private static void Print(string src, ParseException ex)
        {
            if (ex.ErrorPosition.HasValue)
            {
                var line = src.Split(Environment.NewLine)[ex.ErrorPosition.Line - 1];
                var pointer = "^".PadLeft(ex.ErrorPosition.Column);
                var buf = new StringBuilder();
                buf.AppendLine(ex.Message);
                buf.AppendLine(line);
                buf.AppendLine(pointer);
                Console.Write(buf.ToString());
                return;
            }

            Console.WriteLine(ex.Message);
        }

        private static void Print(string src, RuntimeException exception)
        {
            var msg = exception.Position.Match(
                pos =>
                {
                    var line = src.Split(Environment.NewLine)[pos.Line - 1];
                    var pointer = "^".PadLeft(pos.Column);
                    var buf = new StringBuilder();
                    buf.Append("Runtime error ");
                    buf.AppendFormat(
                        "(line {0}, column {1}): ",
                        pos.Line,
                        pos.Column);
                    buf.AppendLine(exception.Message);
                    buf.AppendLine(line);
                    buf.AppendLine(pointer);
                    return buf.ToString();
                },
                () => $"Runtime error: {exception.Message}\n");

            Console.Write(msg);
        }

        private static string Stringify(IValue value)
        {
            if (value == null)
            {
                return "nil";
            }

            switch (value)
            {
                case Value.Boolean x:
                    return x.Value.ToString(Config.CultureInfo).ToLower();
                case Value.Float x:
                    return x.Value.ToString(Config.CultureInfo);
                default:
                    return value.ToString();
            }
        }
    }
}
