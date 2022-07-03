using Itadakimasu.API.Products.Services;
using Itadakimasu.Products.DAL;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(
    optionsBuilder => optionsBuilder.UseNpgsql(
        connectionString,
        b => b.MigrationsAssembly("Itadakimasu.Products.Migrations")));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<MerchandiserService>();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

var env = app.Environment;
if (env.IsDevelopment())
    app.MapGrpcReflectionService();

app.Run();