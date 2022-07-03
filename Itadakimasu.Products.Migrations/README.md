# Add a new migration

Change the current directory to migration's directory:
> cd .\Itadakimasu.Products.Migrations\

To add a new migration just use the next command:
> dotnet ef migrations add AddProducts --context AppDbContext --startup-project ..\Itadakimasu.API.Products
> dotnet ef database update --context AppDbContext --startup-project ..\Itadakimasu.API.Products
