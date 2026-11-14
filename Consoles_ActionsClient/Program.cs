
using Console_CommonModel;

using RootConsumer.ActionsClient;

namespace Consoles_ActionsClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CommonProgram.Run<Program>(args);
            ActionsProgram.RunActionsProgram(ref args);
        }
    }
}
