namespace Oxi.Tool
{
    using System;
    using System.Linq;
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
                    var ast = Oxi.TokenParsers.Expr.Parse(list);

                    var printer = new PrettyPrinter();
                    Console.WriteLine($": {ast.Accept(printer)}");

                    var result = interpreter.Eval(ast);
                    Console.WriteLine($"=> {Stringify(result)}");
                }
                catch (RuntimeException ex)
                {
                    WriteLine(ex);
                }
                catch (ParseException ex)
                {
                    WriteLine(ex);
                }
            }
        }

        private static void WriteLine(ParseException ex) =>
            Console.WriteLine(ex.Message);

        private static void WriteLine(RuntimeException exception)
        {
            var msg = exception.Position.Match(
                pos => $"Runtime error (line {pos.Line}, column {pos.Column}): {exception.Message}",
                () => exception.Message);

            Console.WriteLine(msg);
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
