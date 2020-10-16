using Acmion.CommunicatorCmsLibrary.Core.Application.Pages.Properties;
using Acmion.CommunicatorCmsLibrary.Core.Application.FileSystem;
using Acmion.CommunicatorCmsLibrary.Core.Helpers;
using Acmion.CommunicatorCmsLibrary.Core.Settings;
using Markdig;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Acmion.CommunicatorCmsLibrary.Core.Application.Pages
{
    public class AppPage
    {
        public static AppPage EmptyAppPage { get; } = new AppPage("/");

        private static char IgnoreContentFileStartingCharacter = '_';
        private static IDeserializer YamlDeserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        private static MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder().UseAutoIdentifiers().Build();

        public string PageUrl { get; set; }
        public string PageAppPath { get; set; }
        public string[] ParameterKeys { get; }

        public AppPageProperties Properties { get; set; } = AppPageProperties.Default;
        public AppPagePropertiesNavItem PropertiesNavItem { get; set; } = AppPagePropertiesNavItem.Default;
        public dynamic PropertiesExtra { get; set; } = 0;

        public List<string> ContentFileAppPaths { get => GetContentFileAppPaths(); }

        private List<AppPage>? _subPages;
        private int _subPagesVisibleCount = -1;

        private List<string>? _contentFileAppPaths;

        public AppPage() : this("/")
        {
        }

        public AppPage(string url)
        {
            if (!url.EndsWith('/'))
            {
                url += '/';
            }

            PageUrl = url;
            PageAppPath = AppUrl.ConvertToAppPath(url);

            ParameterKeys = GetParameterKeys(url);
        }

        public static bool IsUrlAppPage(string url) 
        {
            return AppUrl.Exists(AppUrl.Join(url, AppPageSettings.IndexFileName));
        }
        public static bool IsAppPathAppPage(string appPath)
        {
            return IsUrlAppPage(AppPath.ConvertToAppUrl(appPath));
        }

        public static AppPageBaseUrlAndParameterValues GetBaseUrlAndParameterValues(string url) 
        {
            var baseUrl = url;

            while (!IsUrlAppPage(baseUrl))
            {
                baseUrl = AppUrl.ParentUrl(baseUrl);
            }

            var parameters = url.Substring(baseUrl.Length, url.Length - baseUrl.Length).Split(AppUrl.Separator, StringSplitOptions.RemoveEmptyEntries);

            return new AppPageBaseUrlAndParameterValues(baseUrl, parameters);
        }

        public static async Task<AppPage> LoadFromUrl(string url)
        {
            if (IsUrlAppPage(url))
            {
                var page = new AppPage(url);

                await page.LoadProperties();

                return page;
            }

            throw new Exception($"The specified url ({url}) is not a valid AppPage.");
        }
        public static async Task<AppPage> LoadFromAppPath(string appPath)
        {
            return await LoadFromUrl(AppPath.ConvertToAppUrl(appPath));
        }
        public static async Task<AppPage> LoadFromBaseUrlAndParameters(AppPageBaseUrlAndParameterValues baseUrlAndParameters) 
        {
            var currentUrl = baseUrlAndParameters.BaseUrl;

            for (var level = 0; level < baseUrlAndParameters.ParameterValues.Length; level++)
            {
                var dirs = AppUrl.GetDirectories(currentUrl);

                var appPageWithParametersUrl = dirs.FirstOrDefault(d => d.Contains(AppPageSettings.UrlParameterSeparator));

                if (appPageWithParametersUrl != null)
                {
                    currentUrl = appPageWithParametersUrl;
                }
                else
                {
                    throw new Exception("No page with baseUrl and parameterCount found.");
                }
            }

            return await LoadFromUrl(currentUrl);
        }

        public static string GetActualAppPageUrl(string url) 
        {
            if (IsUrlAppPage(url)) 
            {
                return url;
            }

            return GetActualAppPageUrl(GetBaseUrlAndParameterValues(url));
        }
        public static string GetActualAppPageUrl(AppPageBaseUrlAndParameterValues baseUrlAndParameters) 
        {
            var currentUrl = baseUrlAndParameters.BaseUrl;

            for (var level = 0; level < baseUrlAndParameters.ParameterValues.Length; level++)
            {
                var dirs = AppUrl.GetDirectories(currentUrl);

                var appPageWithParametersUrl = dirs.FirstOrDefault(d => d.Contains(AppPageSettings.UrlParameterSeparator));

                if (appPageWithParametersUrl != null)
                {
                    currentUrl = appPageWithParametersUrl;
                }
                else
                {
                    return baseUrlAndParameters.BaseUrl;
                }
            }

            if (!currentUrl.EndsWith(AppUrl.Separator)) 
            {
                currentUrl += AppUrl.Separator;
            }

            return currentUrl;
        }

        public async Task Reload()
        {
            _subPages = null;
            _contentFileAppPaths = null;

            await LoadProperties();
        }

        public async Task<string> Render(RazorPageBase razorPage, IHtmlHelper htmlHelper)
        {
            return await Render(razorPage, htmlHelper, null!);
        }
        public async Task<string> Render(RazorPageBase razorPage, IHtmlHelper htmlHelper, object model)
        {
            var contentFileAppPaths = GetContentFileAppPaths();

            foreach (var cfap in contentFileAppPaths)
            {
                if (cfap.EndsWith(".cshtml"))
                {
                    await htmlHelper.RenderPartialAsync(cfap, model);
                }
                else
                {
                    var content = await AppFile.ReadAllTextAsync(cfap);

                    if (cfap.EndsWith(".md"))
                    {
                        razorPage.Output.Write(Markdown.ToHtml(content, MarkdownPipeline));
                    }
                    else
                    {
                        razorPage.Output.Write(content);
                    }
                }
            }

            return "";
        }

        public bool HasParentPage() 
        {
            if (PageUrl == AppUrl.SeparatorString) 
            {
                return false;
            }

            return IsUrlAppPage(AppPath.GetDirectoryName(PageUrl));
        }
        public async Task<AppPage> GetRootPage() 
        {
            var c = App.Settings.RootPageUrls.Length;

            for (var i = 0; i < c; i++) 
            {
                var rootPageUrl = App.Settings.RootPageUrls[i];

                if (PageUrl.StartsWith(rootPageUrl)) 
                {
                    return await App.Pages.GetByUrl(rootPageUrl);
                }
            }

            return await App.Pages.GetByUrl("/");
        }
        public async Task<AppPage> GetParentPage()
        {
            if (PageUrl.EndsWith(AppUrl.Separator)) 
            {
                return await App.Pages.GetByUrl(AppPath.GetDirectoryName(AppPath.GetDirectoryName(PageUrl)));
            }

            return await App.Pages.GetByUrl(AppPath.GetDirectoryName(PageUrl));
        }

        public async Task<List<AppPage>> GetSubPages()
        {
            if (_subPages == null)
            {
                var subPageAppPaths = GetSubPageAppPaths();

                _subPages = new List<AppPage>(subPageAppPaths.Count);

                foreach (var subPagePath in subPageAppPaths)
                {
                    var subPage = await App.Pages.GetByAppPath(subPagePath);

                    _subPages.Add(subPage);
                }
            }

            return _subPages;
        }
        public async Task<int> GetSubPagesVisibleCount() 
        {
            if (_subPagesVisibleCount == -1) 
            {
                _subPagesVisibleCount = 0;

                var subPages = await GetSubPages();

                foreach (var sp in subPages)
                {
                    if (sp.Properties.ShowInNavigationMenus) 
                    {
                        _subPagesVisibleCount++;
                    }
                }
            }

            return _subPagesVisibleCount;
        }

        private async Task LoadProperties() 
        {
            var propertiesFilePath = AppPath.Join(PageAppPath, AppPageSettings.PropertiesFileName);
            var propertiesLayoutFilePath = AppPath.Join(PageAppPath, AppPageSettings.PropertiesNavItemFileName);
            var propertiesExtraFilePath = AppPath.Join(PageAppPath, AppPageSettings.PropertiesExtraFileName);

            var properties = AppPageProperties.Default;

            if (AppFile.Exists(propertiesFilePath))
            {
                var propertiesCandidate = YamlDeserializer.Deserialize<AppPageProperties>(await AppFile.ReadAllTextAsync(propertiesFilePath));
                if (propertiesCandidate != null)
                {
                    properties = propertiesCandidate;
                }
            }

            var propertiesNavItem = AppPagePropertiesNavItem.Default;

            if (AppFile.Exists(propertiesLayoutFilePath))
            {
                var propertiesNavItemCandidate = YamlDeserializer.Deserialize<AppPagePropertiesNavItem>(await AppFile.ReadAllTextAsync(propertiesLayoutFilePath));
                if (propertiesNavItemCandidate != null)
                {
                    propertiesNavItem = propertiesNavItemCandidate;
                }
            }

            var propertiesExtra = new ExpandoObject();

            if (AppFile.Exists(propertiesExtraFilePath))
            {
                var propertiesExtraCandidate = YamlDeserializer.Deserialize<ExpandoObject>(await AppFile.ReadAllTextAsync(propertiesExtraFilePath));
                if (propertiesExtraCandidate != null)
                {
                    propertiesExtra = propertiesExtraCandidate;
                }
            }

            Properties = properties;
            PropertiesNavItem = propertiesNavItem;
            PropertiesExtra = propertiesExtra;
        }

        private string[] GetParameterKeys(string url)
        {
            var indexOfUrlParameterSeparator = url.IndexOf(AppPageSettings.UrlParameterSeparator);

            if (indexOfUrlParameterSeparator == -1)
            {
                return new string[0];
            }

            var baseUrl = url.Substring(0, indexOfUrlParameterSeparator + 1);

            var parameters = url.Substring(baseUrl.Length - 1, url.Length - baseUrl.Length + 1)
                                .Replace(AppPageSettings.UrlParameterSeparator, AppUrl.SeparatorString)
                                .Replace(AppPageSettings.UrlParameterSeparatorEnd, AppUrl.SeparatorString)
                                .Split(AppUrl.Separator, StringSplitOptions.RemoveEmptyEntries);

            return parameters;
        }

        private List<string> GetSubPageAppPaths()
        {
            var subPageAppPaths = new List<string>(Properties.SubPageOrder.Count);
            var subPageAppPathsSet = new HashSet<string>(Properties.SubPageOrder.Count);
            var subPageAppPathsAfterEllipsis = new List<string>(Properties.SubPageOrder.Count);

            var currentSubPageAppPathList = subPageAppPaths;

            foreach (var spo in Properties.SubPageOrder)
            {
                var subDirectoryPath = AppPath.Join(PageAppPath, spo);

                if (spo == AppPageSettings.SubPageOrderEllipsisIdentifier)
                {
                    currentSubPageAppPathList = subPageAppPathsAfterEllipsis;
                }
                else if (AppDirectory.Exists(subDirectoryPath) && IsAppPathAppPage(subDirectoryPath))
                {
                    currentSubPageAppPathList.Add(subDirectoryPath);
                    subPageAppPathsSet.Add(subDirectoryPath);
                }
            }

            var subDirectories = AppDirectory.GetDirectories(PageAppPath);

            foreach (var subDirectoryPath in subDirectories)
            {
                if (!subPageAppPathsSet.Contains(subDirectoryPath) && IsAppPathAppPage(subDirectoryPath))
                {
                    subPageAppPaths.Add(subDirectoryPath);
                }
            }

            subPageAppPaths.AddRange(subPageAppPathsAfterEllipsis);

            return subPageAppPaths;
        }
        private List<string> GetContentFileAppPaths()
        {
            if (_contentFileAppPaths == null)
            {
                var contentFilePathsSet = new HashSet<string>(Properties.ContentOrder.Count);

                _contentFileAppPaths = new List<string>(Properties.ContentOrder.Count);

                foreach (var co in Properties.ContentOrder)
                {
                    var contentFilePath = Path.Join(PageAppPath, co);

                    if (AppFile.Exists(contentFilePath))
                    {
                        _contentFileAppPaths.Add(contentFilePath);
                        contentFilePathsSet.Add(contentFilePath);
                    }
                }
                var allContentFilePaths = AppDirectory.GetFiles(PageAppPath);
                Array.Sort(allContentFilePaths);

                foreach (var contentFilePath in allContentFilePaths)
                {
                    var fileName = Path.GetFileName(contentFilePath);

                    if (!fileName.StartsWith(IgnoreContentFileStartingCharacter))
                    {
                        if (!contentFilePathsSet.Contains(contentFilePath))
                        {
                            _contentFileAppPaths.Add(contentFilePath);
                        }
                    }
                }
            }

            return _contentFileAppPaths;
        }

        public override string ToString()
        {
            return $"Title: {Properties.Title}, Url: {PageUrl}";
        }
    }
}
