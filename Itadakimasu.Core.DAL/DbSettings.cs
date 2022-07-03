namespace Itadakimasu.Core.DAL;

public record DbSettings
{
    public string ConnectionString { get; init; } = null!;
}