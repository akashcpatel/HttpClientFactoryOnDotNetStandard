using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IRestClient
    {
        Task<RestCallResponse<T>> Get<T>(string endpoint, [CallerMemberName] string callingMethodName = null);

        Task<RestCallResponse<T>> Post<T>(string endpoint, object postData, [CallerMemberName] string callingMethodName = null);

        Task<RestCallResponse<T>> Put<T>(string endpoint, object putData, [CallerMemberName] string callingMethodName = null);

        Task<RestCallResponse<T>> Delete<T>(string endpoint, [CallerMemberName] string callingMethodName = null);
    }
}
