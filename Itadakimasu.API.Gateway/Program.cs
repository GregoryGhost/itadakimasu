using Itadakimasu.API.Gateway;

using Merchandiser.V1;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddGraphQLServer()
       .AddQueryType<Query>();

builder.Services.AddGrpcClient<Merchandiser.V1.Merchandiser.MerchandiserClient>(
       options =>
       {
              var merchandiserAddress = Environment.GetEnvironmentVariable("MERCHANDISER_ADDRESS");
              if (string.IsNullOrWhiteSpace(merchandiserAddress))
              {
                     throw new Exception("You must provide correct merchandiser address.");
              }
              
              options.Address = new Uri(merchandiserAddress);
       }).ConfigureChannel(
       options =>
       {
              options.UnsafeUseInsecureChannelCallCredentials = true;
       });

var app = builder.Build();

app.MapGraphQL();

app.Run();