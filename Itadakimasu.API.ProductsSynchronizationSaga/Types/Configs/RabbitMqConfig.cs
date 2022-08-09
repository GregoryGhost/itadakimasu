namespace Itadakimasu.API.ProductsSynchronizationSaga.Types.Configs;

public record RabbitMqConfig
{
    public string Address { get; init; } = null!;
    public string Login { get; init; } = null!;
    public string Password { get; init; } = null!;
}