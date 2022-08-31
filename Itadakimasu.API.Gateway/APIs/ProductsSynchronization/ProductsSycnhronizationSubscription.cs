namespace Itadakimasu.API.Gateway.APIs.ProductsSynchronization;

using Itadakimasu.API.Gateway.DTOs.ProductsSynchronization;

using JetBrains.Annotations;

[PublicAPI]
[ExtendObjectType(Name = "Subscription")]
public class ProductsSycnhronizationSubscription
{
    [Subscribe]
    public CreatedProductsSynchronizationRequestDto CreatedProductsSynchronizationRequest(
        [EventMessage] CreatedProductsSynchronizationRequestDto createdRequest) => createdRequest;

    [Subscribe]
    public ScrappedProductsSynchronizationRequestDto ScrappedProductsSynchronizationRequest(
        [EventMessage] ScrappedProductsSynchronizationRequestDto scrappedRequest) => scrappedRequest;

    [Subscribe]
    public SynchronizingProductsRequestDto SynchronizingProductsRequest(
        [EventMessage] SynchronizingProductsRequestDto synchronizingRequest) => synchronizingRequest;    
    
    [Subscribe]
    public SynchronizedProductsRequestDto SynchronizedProductsRequest(
        [EventMessage] SynchronizedProductsRequestDto synchronizedRequest) => synchronizedRequest;
}