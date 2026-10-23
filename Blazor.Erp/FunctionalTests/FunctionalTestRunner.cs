using System.Diagnostics;
using System.Text.Json;

namespace BlazorErp.FunctionalTests;

public sealed class FunctionalTestRunner(IEnumerable<IFunctionalTest> tests, ILogger<FunctionalTestRunner> logger)
{
    public async Task<FunctionalTestRunReport> RunAllAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var context = new FunctionalTestContext(services, logger);
        var results = new List<FunctionalTestResult>();

        foreach (var test in tests.OrderBy(t => t.Id))
        {
            var sw = Stopwatch.StartNew();
            try
            {
                logger.LogInformation("Функциональный тест {Id}: {Name}...", test.Id, test.Name);
                await test.ExecuteAsync(context, cancellationToken);
                sw.Stop();
                results.Add(new FunctionalTestResult
                {
                    Id = test.Id,
                    Name = test.Name,
                    Category = test.Category,
                    Passed = true,
                    Duration = sw.Elapsed
                });
                logger.LogInformation("✓ {Id} ({Elapsed}ms)", test.Id, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.LogError(ex, "✗ {Id}: {Message}", test.Id, ex.Message);
                results.Add(new FunctionalTestResult
                {
                    Id = test.Id,
                    Name = test.Name,
                    Category = test.Category,
                    Passed = false,
                    ErrorMessage = ex.Message,
                    Duration = sw.Elapsed
                });
            }
        }

        return new FunctionalTestRunReport { Results = results };
    }

    public static string SerializeResults(IReadOnlyList<FunctionalTestResult> results) =>
        JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = false });
}
