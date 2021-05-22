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
        public string Query { get; private set; } = "";
        public string[] PartialUrls { get; private set; } = new string[] { "/" };
        public string[] ParameterValues { get; private set; } = new string[0];
        public Dictionary<string, string> ParameterDictionary { get; private set; } = new Dictionary<string, string>();

        public AppPage CurrentAppPage { get; private set; } = null!;
        public AppPage CurrentRootAppPage { get; private set; } = null!;

        public Language Language { get; set; } = null!;
        public Breadcrumb Breadcrumb { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string LogoIcon { get; set; } = null!;
        public string LogoContent { get; set; } = null!;
        public string Theme { get; set; } = null!;


        public string[] ParameterKeys => CurrentAppPage.ParameterKeys;

        private IHttpContextAccessor _httpContextAccessor;

        public RequestState(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Initialize(string url, string query, string[] parameterValues, AppPage currentAppPage) 
        {
            Url = url;
            Query = query;
            PartialUrls = AppUrl.AllPartialUrls(url);
            ParameterValues = parameterValues;

            for (var i = 0; i < parameterValues.Length; i++)
            {
                var key = currentAppPage.ParameterKeys[i];
                var val = parameterValues[i];

                ParameterDictionary[key] = val;
            }

            CurrentAppPage = currentAppPage;
            CurrentRootAppPage = await currentAppPage.GetRootPage();

            Language = GetLanguage();
            Breadcrumb = await Breadcrumb.GetFromAppPage(currentAppPage);

            Title = Breadcrumb.LastBreadcrumbItem.Title;
            Description = CurrentAppPage.Properties.Description;

            LogoIcon = App.Settings.LogoIcon;
            LogoContent = App.Settings.LogoContent;
            Theme = App.Settings.Theme;

        }

        public async Task<AppPage> GetCurrentPage() 
        {
            return await App.Pages.GetByUrl(Url);
        }

        public string Translate(string key) 
        {
            return App.TranslationHandler.Translate(key, Language.Id);
        }

        public string TranslateUrl(string languageId) 
        {
            var translatedUrl = App.Settings.TranslateUrl(CurrentAppPage.PageUrl, languageId);

            if (CurrentAppPage.HasParameters) 
            {
                foreach (var parameterKey in ParameterKeys)
                {
                    translatedUrl = translatedUrl.Replace("[" + parameterKey + "]", ParameterDictionary[parameterKey]);
                }
            }

            return translatedUrl;
        }
        public string TranslateUrl(Language language) 
        {
            return TranslateUrl(language.Id);
        }

        private Language GetLanguage() 
        {
            var separatorIndex = Url.IndexOf(AppUrl.Separator, 1);

            if (separatorIndex == -1) 
            {
                return Language.Default;
            }

            var languageId = Url.Substring(1, separatorIndex - 1);

            var lang = App.Settings.Languages.FirstOrDefault(lang => lang.Id == languageId);

            if (lang == null) 
            {
                return Language.Default;
            }

            return lang;
        }
    }
}
