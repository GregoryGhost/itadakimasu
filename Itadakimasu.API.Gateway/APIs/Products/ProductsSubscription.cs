namespace Itadakimasu.API.Gateway.APIs.Products;

using Itadakimasu.API.Gateway.DTOs.Products;

using JetBrains.Annotations;

[PublicAPI]
[ExtendObjectType(Name = "Subscription")]
public class ProductsSubscription
{
    [Subscribe]
    public CreatedProductDto ProductAdded([EventMessage] CreatedProductDto product) => product;
}