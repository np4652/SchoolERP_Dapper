using Entities.Enums;
using System.ComponentModel;

namespace Data
{
    public interface IResponse<T>
    {
        ResponseStatus StatusCode { get; set; }
        string ResponseText { get; set; }
        T Result { get; set; }
    }

    public interface IResponse
    {
        ResponseStatus StatusCode { get; set; }
        string ResponseText { get; set; }
    }

    public interface IRequest<T>
    {
        string AuthToken { get; set; }
        T Param { get; set; }
    }

    public class Response<T> : IResponse<T>
    {
        public ResponseStatus StatusCode { get; set; }
        public string ResponseText { get; set; }
        //public Exception Exception { get; set; }
        public T Result { get; set; }
        public Dictionary<string, string> KeyVals { get; set; }

        public Response()
        {
            StatusCode = ResponseStatus.Failed;
            ResponseText = ResponseStatus.Failed.ToString();
        }
    }

    public class Response : IResponse
    {
        public ResponseStatus StatusCode { get; set; } = ResponseStatus.Failed;
        public string ResponseText { get; set; } = ResponseStatus.Failed.ToString();
        public Response()
        {
            StatusCode = ResponseStatus.Failed;
            ResponseText = ResponseStatus.Failed.ToString();
        }
    }

    public class ResponseForUnauthorized
    {
        [DefaultValue("Unauthorized")]
        public string message { get; set; }
    }
    public class Request<T> : IRequest<T>
    {
        public string AuthToken { get; set; }
        public T Param { get; set; }
    }

    public class PagedResult
    {
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class PagedResult<T> : PagedResult where T : class
    {
        public PagedResult()
        {

        }
        public List<T> Data { get; set; }
    }

    public class PagedRequest : PagedResult
    {
        public int LoginId { get; set; }
        public dynamic Param { get; set; }
    }
}
