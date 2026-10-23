public static class GridViewExtensions
{

    public static bool IsVerticalResizable(this Control p)
    {
        int h = p.Height;
        p.Height += 1;
        int h2 = p.Height;
        p.Height -= 1;
        return h != h2;
    }

    public static string GetPath(this Control p)
    {
        string res = "/";
        foreach (var next in p.GetParents().Select(p => p.Name).ToList())
        {
            res += $"{next}/";
        }
        return res;
    }

    public static void ForEach(this Control p, Action<Control> todo)
    {
        //System.Console.WriteLine(p.GetPath());
        todo(p);
        foreach (Control child in p.Controls)
        {
            child.ForEach(todo);
        }
    }
}
