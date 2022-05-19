using System.Net;

namespace Repositories
{
    public class RestCallResponse<T>
    {
        public T Data { get; set; } = default;

        public HttpStatusCode ResponseCode { get; internal set; }
    }
}
