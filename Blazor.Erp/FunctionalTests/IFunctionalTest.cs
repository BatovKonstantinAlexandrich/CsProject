namespace BlazorErp.FunctionalTests;

public interface IFunctionalTest
{
    string Id { get; }
    string Name { get; }
    string Category { get; }
    Task ExecuteAsync(FunctionalTestContext context, CancellationToken cancellationToken = default);
}
