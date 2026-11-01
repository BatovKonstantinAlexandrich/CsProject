using RootConsumer.ActionsClient;

namespace Consoles_ActionsClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            ActionsProgram.RunActionsProgram(ref args);
        }
    }
}
