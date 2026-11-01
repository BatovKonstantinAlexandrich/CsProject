namespace ConsoleControl
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            args = new string[] { "Новая игра" };
            IConsoleСontrol control = new InputConsoleСontrol(ref args);
            control.UserInteractive = args.Length == 0;

            control.SelectAction
            (
                "Меню",
                new List<string>() 
                { 
                    "Новая игра",
                    "Выход"
                },
                ref args
            );

            Console.WriteLine(args);
        }
    }
}
