namespace ProductScrapper.Services;

public class MrTakoClient: HttpClient
{
    public MrTakoClient() : this(new HttpClientHandler())
    {
    }

    protected MrTakoClient(HttpMessageHandler handler, bool disposeHandler = true) : base(handler, disposeHandler)
    {
    }
}