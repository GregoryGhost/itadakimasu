namespace Itadakimasu.API.Gateway.DTOs.Pagination;

using JetBrains.Annotations;

[UsedImplicitly]
public record PaginationSettings
{
    [GraphQLType(typeof(UnsignedIntType))]
    public uint Skip { get; init; }
    
    [GraphQLType(typeof(UnsignedIntType))]
    public uint Take { get; init; }
}