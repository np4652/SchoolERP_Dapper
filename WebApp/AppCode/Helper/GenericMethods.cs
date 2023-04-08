using AppUtility.APIRequest;
using Data;
using Entities;
using Newtonsoft.Json;
using System.Net;

namespace WebApp.AppCode.Helper
{
    public interface IGenericMethods
    {
        Task<T> Get<T>(string URL, string Token, dynamic P = null);
        Task<IEnumerable<T>> GetList<T>(string URL, string Token, dynamic P = null);
    }
    public class GenericMethods : IGenericMethods
    {
        private string _apiBaseURL;
        public GenericMethods(AppSettings appSettings)
        {
            _apiBaseURL = appSettings.WebAPIBaseUrl;
        }

        public async Task<T> Get<T>(string URL, string Token, dynamic P = null)
        {
            T result = default(T);
            string data = P == null ? "" : JsonConvert.SerializeObject(P);
            var apiResponse = await AppWebRequest.O.PostAsync($"{_apiBaseURL}/api/{URL}", data, Token);
            if (apiResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                var deserializeObject = JsonConvert.DeserializeObject<Response<T>>(apiResponse.Result);
                result = deserializeObject.Result;
                return result;
            }
            return result;
        }
        public async Task<IEnumerable<T>> GetList<T>(string URL, string Token, dynamic P = null)
        {
            IEnumerable<T> result = default(IEnumerable<T>);
            string data = P == null ? "" : JsonConvert.SerializeObject(P);
            var apiResponse = await AppWebRequest.O.PostAsync($"{_apiBaseURL}/api/{URL}", data, Token);
            if (apiResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                var deserializeObject = JsonConvert.DeserializeObject<Response<IEnumerable<T>>>(apiResponse.Result);
                result = deserializeObject.Result;
                return result;
            }
            return result;
        }
    }
}
