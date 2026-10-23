namespace BlazorErp.FunctionalTests;

public sealed class FunctionalTestResult
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public bool Passed { get; init; }
    public string? ErrorMessage { get; init; }
    public TimeSpan Duration { get; init; }
}

public sealed class FunctionalTestRunReport
{
    public required IReadOnlyList<FunctionalTestResult> Results { get; init; }
    public int PassedCount => Results.Count(r => r.Passed);
    public int TotalCount => Results.Count;
    public bool AllPassed => PassedCount == TotalCount;
}

public sealed class AppVersionInfo
{
    public int ReleaseCount { get; init; }
    public int PassedTests { get; init; }
    public int TotalTests { get; init; }
    public int BuildNumber { get; init; }
    public DateTime LastRunAt { get; init; }
    public bool AllTestsPassed { get; init; }

    public string Formatted => $"{ReleaseCount}.{PassedTests}/{TotalTests} {BuildNumber}";
}
