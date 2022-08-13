using Itadakimasu.API.Gateway.APIs.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddGraphQLServer()
       .AddQueryType<ProductsQuery>()
       .AddMutationType<ProductsMutation>();

builder.Services.AddGrpcClient<Merchandiser.V1.Merchandiser.MerchandiserClient>(
           options =>
           {
               var merchandiserAddress = Environment.GetEnvironmentVariable("API_PRODUCTS_ADDRESS");
               if (string.IsNullOrWhiteSpace(merchandiserAddress))
                   throw new Exception("You must provide correct merchandiser address.");

               options.Address = new Uri(merchandiserAddress);
           })
       .ConfigureChannel(
           options => { options.UnsafeUseInsecureChannelCallCredentials = true; });

var app = builder.Build();

app.MapGraphQL();

app.Run();