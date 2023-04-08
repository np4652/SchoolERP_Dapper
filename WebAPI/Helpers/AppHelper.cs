using Microsoft.Extensions.Primitives;
using System.Net;
using System.Text;

namespace WebAPI.Helpers
{
    public class AppHelper
    {
        public static AppHelper O => instance.Value;
        private static Lazy<AppHelper> instance = new Lazy<AppHelper>(() => new AppHelper());
        private AppHelper() { }
        public async Task<HttpContextRequest> ExtractHttpContextRequestAsync(HttpRequest request, bool isUrlDecodeNeeded = true)
        {
            var response = new HttpContextRequest
            {
                Method = request.Method,
                Path = request.Path,
                Scheme = request.Scheme
            };
            StringBuilder resp = new StringBuilder("");
            try
            {
                if (request.Method == "POST")
                {
                    if (request.HasFormContentType)
                    {
                        if (request.Form.Keys.Count > 0)
                        {
                            foreach (var item in request.Form.Keys)
                            {
                                request.Form.TryGetValue(item, out StringValues strVal);
                                if (resp.Length == 0)
                                {
                                    resp.AppendFormat("{0}={1}", item, strVal);
                                }
                                else
                                {
                                    resp.AppendFormat("&{0}={1}", item, strVal);
                                }
                            }
                        }
                    }
                    else
                    {
                        resp = new StringBuilder(await request.GetRawBodyStringAsync().ConfigureAwait(false));
                    }
                }
                else
                {
                    if (request.HasFormContentType)
                    {
                        if (request.Form.Keys.Count > 0)
                        {
                            foreach (var item in request.Form.Keys)
                            {
                                request.Form.TryGetValue(item, out StringValues strVal);
                                if (resp.Length == 0)
                                {
                                    resp.AppendFormat("{0}={1}", item, strVal);
                                }
                                else
                                {
                                    resp.AppendFormat("&{0}={1}", item, strVal);
                                }
                            }
                        }
                    }
                    else
                    {
                        resp = new StringBuilder(await request.GetRawBodyStringAsync().ConfigureAwait(false));
                    }
                }
                if (resp.Length == 0)
                {
                    resp = new StringBuilder(request.QueryString.ToString());
                }
                response.Content = isUrlDecodeNeeded ? WebUtility.UrlDecode(resp.ToString()) : resp.ToString();
            }
            catch (Exception ex)
            {
                response.Content= ex.Message;
            }
            return response;
        }
    }

    public class HttpContextRequest
    {
        public string Method { get; set; }
        public string Content { get; set; }
        public string Scheme { get; set; }
        public string Path { get; set; }
        public string RemoteIP { get; set; }
    }
}
