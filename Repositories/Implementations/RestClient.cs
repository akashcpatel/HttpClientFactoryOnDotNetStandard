using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class RestClient : IRestClient
    {
        private readonly object _getClientLockObject = new object();
        private HttpClient _httpClient;
        private HttpClient HttpClient
        {
            get
            {
                lock (_getClientLockObject)
                {
                    if (_httpClient == null)
                        _httpClient = new HttpClient(new HttpClientHandler { UseCookies = false });
                }

                return _httpClient;
            }
        }

        public async Task<RestCallResponse<T>> Get<T>(string endpoint, [CallerMemberName] string callingMethodName = null) =>
            await Execute<T>(RestCallData.Create(HttpMethod.Get, endpoint, callingMethodName));

        public async Task<RestCallResponse<T>> Post<T>(string endpoint, object postData, [CallerMemberName] string callingMethodName = null) =>
            await Execute<T>(RestCallData.Create(HttpMethod.Post, endpoint, postData, callingMethodName));

        public async Task<RestCallResponse<T>> Put<T>(string endpoint, object putData, [CallerMemberName] string callingMethodName = null) =>
            await Execute<T>(RestCallData.Create(HttpMethod.Put, endpoint, putData, callingMethodName));

        public async Task<RestCallResponse<T>> Delete<T>(string endpoint, [CallerMemberName] string callingMethodName = null) =>
            await Execute<T>(RestCallData.Create(HttpMethod.Delete, endpoint, callingMethodName));

        private async Task<RestCallResponse<T>> Execute<T>(RestCallData restCallData)
        {
            var httpRequest = restCallData.CreateHttpRequest();
            var httpResponse = await HttpClient.SendAsync(httpRequest);
            httpResponse.EnsureSuccessStatusCode();
            return await CreateRestCallResponse<T>(httpResponse);
        }

        private async Task<RestCallResponse<T>> CreateRestCallResponse<T>(HttpResponseMessage httpResponse)
        {
            return new RestCallResponse<T>
            {
                ResponseCode = httpResponse.StatusCode,
                Data = (httpResponse.StatusCode == HttpStatusCode.OK) ?
                            JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync()) :
                            default
            };
        }

        ~RestClient()
        {
            _httpClient?.CancelPendingRequests();
            _httpClient?.Dispose();
            _httpClient = null;
        }

        public class RestCallData
        {
            public HttpMethod Method { get; set; }
            public string Endpoint { get; set; }
            public object SendData { get; set; } = null;
            public string CallingMethodName { get; set; }

            public static RestCallData Create(HttpMethod httpMethod, string endpoint, string callingMethodName) =>
                new RestCallData
                {
                    Method = httpMethod,
                    Endpoint = endpoint,
                    CallingMethodName = callingMethodName
                };

            public static RestCallData Create(HttpMethod httpMethod, string endpoint, object sendData, string callingMethodName) =>
                new RestCallData
                {
                    Method = httpMethod,
                    Endpoint = endpoint,
                    SendData = sendData,
                    CallingMethodName = callingMethodName
                };

            public HttpRequestMessage CreateHttpRequest()
            {
                var httpRequestMessage = new HttpRequestMessage(Method, Endpoint);
                if(SendData != null)
                     httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(SendData), Encoding.UTF8, "application/json");
                return httpRequestMessage;
            }
        }
    }    
}
