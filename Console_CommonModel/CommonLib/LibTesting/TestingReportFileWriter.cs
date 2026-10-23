using System.Text;

namespace Console_CommonModel.CommonLib.LibTesting;

/// <summary>
/// Сохранение итогового отчёта тестирования в текстовый файл для анализа.
/// </summary>
public static class TestingReportFileWriter
{
    public static string DefaultDirectory =>
        Path.Combine(Directory.GetCurrentDirectory(), "TestingReports");

    /// <summary>
    /// Записывает отчёт в каталог <see cref="DefaultDirectory"/> (и копию TestingReport_latest.txt).
    /// </summary>
    public static string Save(TestingReport report, string? directory = null)
    {
        directory ??= DefaultDirectory;
        Directory.CreateDirectory(directory);

        var stamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
        var path = Path.Combine(directory, $"TestingReport_{stamp}.txt");
        var latestPath = Path.Combine(directory, "TestingReport_latest.txt");
        var content = BuildFullReport(report);

        File.WriteAllText(path, content, Encoding.UTF8);
        File.WriteAllText(latestPath, content, Encoding.UTF8);
        return path;
    }

    public static string BuildSummary(TestingReport report)
    {
        var leaves = CollectLeaves(report, report.Name ?? "Root");
        int passed = leaves.Count(l => !l.Failed);
        int failed = leaves.Count(l => l.Failed);
        int notImplemented = leaves.Count(l => l.LooksNotImplemented);

        var sb = new StringBuilder();
        sb.AppendLine("--- Сводка ---");
        sb.AppendLine($"Листовых тестов: {leaves.Count}");
        sb.AppendLine($"Успех: {passed}");
        sb.AppendLine($"Провал: {failed}");
        if (notImplemented > 0)
            sb.AppendLine($"Без реализации / заглушки: {notImplemented}");
        return sb.ToString();
    }

    public static string BuildFullReport(TestingReport report)
    {
        var leaves = CollectLeaves(report, report.Name ?? "Root");
        var sb = new StringBuilder();

        sb.AppendLine("=== Отчёт тестирования CommonModel ===");
        sb.AppendLine($"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Рабочая папка: {Directory.GetCurrentDirectory()}");
        sb.AppendLine();
        sb.Append(BuildSummary(report));

        var failedLeaves = leaves.Where(l => l.Failed).ToList();
        if (failedLeaves.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("--- Проваленные тесты (листовые) ---");
            foreach (var leaf in failedLeaves)
            {
                sb.AppendLine($"* {leaf.Path}");
                AppendMessages(sb, leaf.Report, "  ");
            }
        }

        var notImpl = leaves.Where(l => l.LooksNotImplemented).ToList();
        if (notImpl.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("--- Не реализовано / NotImplementedException ---");
            foreach (var leaf in notImpl)
            {
                sb.AppendLine($"* {leaf.Path}");
                AppendMessages(sb, leaf.Report, "  ");
            }
        }

        sb.AppendLine();
        sb.AppendLine("--- Дерево отчётов ---");
        sb.Append(FormatTree(report, 0, isRoot: true));

        sb.AppendLine();
        sb.AppendLine("--- JSON (краткая сериализация корня) ---");
        sb.AppendLine(report.ToString());

        return sb.ToString();
    }

    private sealed record LeafInfo(string Path, TestingReport Report, bool Failed, bool LooksNotImplemented);

    private static List<LeafInfo> CollectLeaves(TestingReport report, string path)
    {
        var list = new List<LeafInfo>();
        CollectLeavesCore(report, path, list);
        return list;
    }

    private static void CollectLeavesCore(TestingReport report, string path, List<LeafInfo> list)
    {
        if (report.SubReports == null || report.SubReports.Count == 0)
        {
            list.Add(new LeafInfo(
                path,
                report,
                report.Failed,
                LooksNotImplemented(report)));
            return;
        }

        foreach (var pair in report.SubReports)
        {
            var childPath = string.IsNullOrWhiteSpace(path)
                ? pair.Key
                : $"{path} → {pair.Key}";
            CollectLeavesCore(pair.Value, childPath, list);
        }
    }

    private static bool LooksNotImplemented(TestingReport report)
    {
        foreach (var message in report.Messages ?? [])
        {
            if (message.Contains("NotImplementedException", StringComparison.OrdinalIgnoreCase)
                || message.Contains("Не реализовано", StringComparison.OrdinalIgnoreCase)
                || message.Contains("не реализован", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return report.Name?.Contains("NotImplemented", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static void AppendMessages(StringBuilder sb, TestingReport report, string indent)
    {
        if (report.Started.HasValue)
            sb.AppendLine($"{indent}Начало: {report.Started:yyyy-MM-dd HH:mm:ss}");
        if (report.Ended.HasValue)
            sb.AppendLine($"{indent}Конец: {report.Ended:yyyy-MM-dd HH:mm:ss}");
        foreach (var message in report.Messages ?? [])
            sb.AppendLine($"{indent}{message}");
    }

    private static string FormatTree(TestingReport report, int level, bool isRoot)
    {
        var sb = new StringBuilder();
        var indent = new string(' ', level * 2);
        var status = report.Failed ? "[Провал]" : "[Успех]";

        if (isRoot)
            sb.AppendLine($"{status} {report.Name}");
        else
            sb.AppendLine($"{indent}{status} {report.Name}");

        foreach (var message in report.Messages ?? [])
            sb.AppendLine($"{indent}  {message}");

        if (report.SubReports == null || report.SubReports.Count == 0)
            return sb.ToString();

        var index = 1;
        foreach (var pair in report.SubReports)
        {
            sb.AppendLine($"{indent}  {index}/{report.SubReports.Count}: {pair.Key}");
            sb.Append(FormatTree(pair.Value, level + 1, isRoot: false));
            index++;
        }

        return sb.ToString();
    }
}
