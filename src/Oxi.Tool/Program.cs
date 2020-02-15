namespace Oxi.Tool
{
    using System;
    using PowerArgs;

    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    class Program
    {
        [HelpHook]
        public bool Help { get; set; }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Args.InvokeAction<Program>(args);
            }
        }
    }
}
