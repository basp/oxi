namespace Oxi.Tool
{
    using System;
    using PowerArgs;
    using Superpower;

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

            var scanner = new Scanner();
            while (true)
            {
                Console.Write("> ");
                var src = Console.ReadLine();
                try
                {
                    var tokens = scanner.Tokenize(src);
                    foreach (var tok in tokens)
                    {
                        Console.WriteLine($"{tok.Kind} (line {tok.Position.Line}, column {tok.Position.Column})");
                    }

                    var res = Oxi.TokenParsers.Expr.Parse(tokens);
                    var printer = new AstPrinter();
                    Console.WriteLine(res.Accept(printer));
                }
                catch (ParseException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
