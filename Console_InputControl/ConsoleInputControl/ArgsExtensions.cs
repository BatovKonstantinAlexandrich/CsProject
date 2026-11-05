public static class ArgsExtensions
{
    public static string Shift(this object p, ref string[] args)
    {
        if (args == null || args.Length == 0)
            throw new InvalidOperationException("Не достаточно аргументов");
        string value = args[0];
        var argsOut = new string[args.Length - 1];
        for (int i = 1; i < args.Length; i++)
            argsOut[i - 1] = args[i];
        args = argsOut;
        return value;
    }

    public static void Push(this object p, ref string[] args, string next)
    {
        if (args == null)
            throw new InvalidOperationException("Не задана ссылка на аргументы");
        var argsOut = new string[args.Length + 1];
        for (int i = 0; i < args.Length; i++)
            argsOut[i] = args[i];
        argsOut[args.Length] = next;
        args = argsOut;
    }


    public static string RemoveFirst(this object target, ref string[] args)
    {
        if (args == null || args.Length == 0) return null;
        string result = args[0];
        args = args.Skip(1).ToArray();  // нужен using System.Linq;
        return result;
    }

    public static string RemoveLast(this object target, ref string[] args)
    {
        if (args == null || args.Length == 0) return null;
        string result = args[args.Length - 1];
        args = args.Take(args.Length - 1).ToArray();
        return result;
    }
}

