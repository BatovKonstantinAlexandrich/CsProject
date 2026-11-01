public static class ArgsExtensions
{
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

