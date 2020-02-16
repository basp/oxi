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

                    var printer = new AstPrinter();
                    Console.WriteLine(ast.Accept(printer));

                    var val = interpreter.Eval(ast);
                    Console.WriteLine($"=> {Interpreter.Stringify(val)}");
                }
                catch (ParseException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
