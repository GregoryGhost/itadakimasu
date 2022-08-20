namespace Itadakimasu.API.Gateway.DTOs.Pagination;

using JetBrains.Annotations;

[UsedImplicitly]
public record PaginationSettings
{
    public uint Skip { get; init; }
    public uint Take { get; init; }
}