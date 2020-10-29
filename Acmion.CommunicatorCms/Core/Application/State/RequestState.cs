using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application;
using Acmion.CommunicatorCms.Core.Application.DataTypes;
using Acmion.CommunicatorCms.Core.Application.FileSystem;
using Acmion.CommunicatorCms.Core.Application.Pages;
using Acmion.CommunicatorCms.Core.Application.Utilities;
using Acmion.CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Acmion.CommunicatorCms.Core
{
    public class RequestState
    {
        public dynamic Dynamic { get; set; } = new ExpandoObject();
        public HttpRequest HttpRequest => _httpContextAccessor.HttpContext.Request;

        // Props initialized in Initialize
        public string Url { get; private set; } = "/";
        public AppPage CurrentAppPage { get; private set; } = null!;

        public dynamic Parameters { get; private set; } = new ExpandoObject();
        public string[] ParameterValues { get; private set; } = new string[0];
        public string[] ParameterKeys => CurrentAppPage.ParameterKeys;
        public Dictionary<string, string> ParameterDictionary { get; private set; } = new Dictionary<string, string>();

        public string Title { get; set; } = null!;
        public string LogoIcon { get; set; } = null!;
        public string LogoContent { get; set; } = null!;
        public string Theme { get; set; } = null!;

        public Language Language { get; set; } = null!;
        public Breadcrumb Breadcrumb { get; set; } = null!;

        private IHttpContextAccessor _httpContextAccessor;

        public RequestState(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Initialize(string url, string[] parameterValues, AppPage currentAppPage) 
        {
            Url = url;
            ParameterValues = parameterValues;
            CurrentAppPage = currentAppPage;

            Breadcrumb = await Breadcrumb.GetFromAppPage(currentAppPage);
            Title = Breadcrumb.LastBreadcrumbItem.Title;

            LogoIcon = App.Settings.LogoIcon;
            LogoContent = App.Settings.LogoContent;
            Theme = App.Settings.Theme;

            Language = GetLanguage();

            var parametersCollection = (ICollection<KeyValuePair<string, object>>)Parameters;

            for (var i = 0; i < parameterValues.Length; i++) 
            {
                // Keys should be reversed, because if only one parameter is specified, then it is the last
                var key = currentAppPage.ParameterKeys[parameterValues.Length - i - 1];
                var val = parameterValues[i];

                ParameterDictionary[key] = val;
                parametersCollection.Add(new KeyValuePair<string, object>(key, val));
            }

        }

        public async Task<AppPage> GetCurrentPage() 
        {
            return await App.Pages.GetByUrl(Url);
        }

        private Language GetLanguage() 
        {
            var separatorIndex = Url.IndexOf(AppUrl.Separator, 1);

            if (separatorIndex == -1) 
            {
                return Language.Unspecified;
            }

            var languageId = Url.Substring(1, separatorIndex - 1);

            var lang = App.Settings.Languages.FirstOrDefault(lang => lang.Id == languageId);

            if (lang == null) 
            {
                return Language.Unspecified;
            }

            return lang;
        }
    }
}
