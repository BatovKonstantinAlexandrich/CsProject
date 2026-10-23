using System.Diagnostics;

/// <summary>
/// Логирование времени формирования представлений и связывания с элементами управления WinForms.
/// </summary>
public static class ViewPerformanceExtensions
{
    private const long SlowOperationMs = 100;

    public static T LogPerf<T>(this object target, string text, Func<T> todo)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return todo();
        }
        finally
        {
            LogDuration(target, text, sw.ElapsedMilliseconds);
        }
    }

    public static void LogPerf(this object target, string text, Action todo)
    {
        target.LogPerf(text, () =>
        {
            todo();
            return true;
        });
    }

    public static string ViewPerfLabel(this ViewItem view)
    {
        var childCount = view.Controls?.Count ?? 0;
        return $"{view.GetTypeName()}#{view.GetHashCode()} (children={childCount})";
    }

    private static void LogDuration(object target, string text, long elapsedMs)
    {
        var suffix = elapsedMs >= SlowOperationMs ? " [МЕДЛЕННО]" : string.Empty;
        target.Info($"[Perf] {text}: {elapsedMs} мс.{suffix}");
    }
}
