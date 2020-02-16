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
                    foreach (var tok in list)
                    {
                        Console.WriteLine($"{tok.Kind} (line {tok.Position.Line}, column {tok.Position.Column})");
                    }

                    var res = Oxi.TokenParsers.Expr.Parse(list);
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
