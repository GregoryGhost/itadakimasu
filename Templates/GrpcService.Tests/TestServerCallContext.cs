namespace GrpcService.Tests;

using Grpc.Core;

public class TestServerCallContext : ServerCallContext
{
    private readonly Dictionary<object, object> _userState;

    private TestServerCallContext(Metadata requestHeaders, CancellationToken cancellationToken)
    {
        RequestHeadersCore = requestHeaders;
        CancellationTokenCore = cancellationToken;
        ResponseTrailersCore = new Metadata();
        AuthContextCore = new AuthContext(string.Empty, new Dictionary<string, List<AuthProperty>>());
        _userState = new Dictionary<object, object>();
    }

    protected override AuthContext AuthContextCore { get; }

    protected override CancellationToken CancellationTokenCore { get; }

    protected override DateTime DeadlineCore { get; }

    protected override string HostCore => "HostName";

    protected override string MethodCore => "MethodName";

    protected override string PeerCore => "PeerName";

    protected override Metadata RequestHeadersCore { get; }

    public Metadata? ResponseHeaders { get; private set; }

    protected override Metadata ResponseTrailersCore { get; }

    protected override Status StatusCore { get; set; }

    protected override IDictionary<object, object> UserStateCore => _userState;

    protected override WriteOptions? WriteOptionsCore { get; set; }

    public static TestServerCallContext Create(Metadata? requestHeaders = null,
        CancellationToken cancellationToken = default)
    {
        return new TestServerCallContext(requestHeaders ?? new Metadata(), cancellationToken);
    }

    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
    {
        throw new NotImplementedException();
    }

    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
    {
        if (ResponseHeaders != null)
            throw new InvalidOperationException("Response headers have already been written.");

        ResponseHeaders = responseHeaders;
        return Task.CompletedTask;
    }
}