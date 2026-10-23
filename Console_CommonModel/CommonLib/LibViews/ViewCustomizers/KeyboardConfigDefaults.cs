namespace Console_CommonModel.CommonLib.LibViews.ViewCustomizers;

/// <summary>Примеры привязок клавиш по умолчанию (как в <c>keyboard.json</c>).</summary>
public static class KeyboardConfigDefaults
{
    public const string DefaultFileName = "keyboard.json";

    public static Dictionary<string, string> CreateSample() => new()
    {
        ["`"] = "SetDesignMode(true)",
        ["0"] = "Close()",
        ["Control+S"] = "Save()",
        ["Control+Shift+S"] = "SaveAs()",
        ["q"] = "Close()",
    };

    public static string ResolveConfigPath(string filename = DefaultFileName)
    {
        if (Path.IsPathRooted(filename))
            return filename;

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var candidate = Path.Combine(baseDir, filename);
        if (File.Exists(candidate))
            return candidate;

        var projectCandidate = Path.Combine(Directory.GetCurrentDirectory(), filename);
        return File.Exists(projectCandidate) ? projectCandidate : candidate;
    }
}
