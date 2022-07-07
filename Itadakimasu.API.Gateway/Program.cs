using Itadakimasu.API.Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddGraphQLServer()
       .AddQueryType<Query>();

builder.Services.AddGrpcClient<Merchandiser.V1.Merchandiser.MerchandiserClient>(
       optins =>
       {
              var merchandiserAddress = Environment.GetEnvironmentVariable("MERCHANDISER_ADDRESS");
              if (string.IsNullOrWhiteSpace(merchandiserAddress))
              {
                     throw new Exception("You must provide correct merchandiser address.");
              }
              optins.Address = new Uri(merchandiserAddress);
       });

var app = builder.Build();

app.MapGraphQL();

app.Run();