namespace RestServer.Application.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content);
        Task<string> GetStringAsync(Uri requestUri);
    }
}
