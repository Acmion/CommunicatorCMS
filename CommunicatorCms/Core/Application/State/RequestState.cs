using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CommunicatorCms.Core.Application;
using CommunicatorCms.Core.Application.FileSystem;
using CommunicatorCms.Core.Application.Pages;
using CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace CommunicatorCms.Core
{
    public class RequestState
    {
        public string Url { get; set; }
        public string ActualUrl { get; set; }

        public dynamic Dynamic { get; set; } = new ExpandoObject();
        public HttpRequest HttpRequest => _httpContextAccessor.HttpContext.Request;

        public bool HasQueryPaths => HttpRequest.Query.ContainsKey(QuerySettings.PathParameter);
        public bool HasQueryTitles => HttpRequest.Query.ContainsKey(QuerySettings.TitleParameter);

        private IHttpContextAccessor _httpContextAccessor;

        public RequestState(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
            ActualUrl = HttpRequest.Path.Value.Replace(AppPageSettings.IndexUrl, "");
            Url = AppUrl.ConvertToVirtualUrl(ActualUrl);
        }

        public async Task<AppPage> GetCurrentPage() 
        {
            return await App.Pages.GetByUrl(Url);
        }

        public StringValues GetQueryPaths() => HttpRequest.Query[QuerySettings.PathParameter];
        public StringValues GetQueryTitles() => HttpRequest.Query[QuerySettings.TitleParameter];

    }
}
