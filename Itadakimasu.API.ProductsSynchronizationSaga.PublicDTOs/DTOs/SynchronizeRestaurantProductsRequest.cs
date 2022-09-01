namespace Itadakimasu.API.ProductsSynchronizationSaga.PublicDTOs.DTOs;

using System;

using JetBrains.Annotations;

using MassTransit;

[PublicAPI]
public record SynchronizeRestaurantProductsRequest : CorrelatedBy<Guid>
{
    public ulong RestaurantId { get; init; }

    public Guid CorrelationId { get; }
}