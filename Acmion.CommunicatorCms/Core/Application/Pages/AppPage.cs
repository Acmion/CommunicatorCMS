using Acmion.CommunicatorCms.Core.Application.Pages.Properties;
using Acmion.CommunicatorCms.Core.Application.FileSystem;
using Acmion.CommunicatorCms.Core.Helpers;
using Acmion.CommunicatorCms.Core.Settings;
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
using Acmion.CommunicatorCms.Core.Application.DataTypes;

namespace Acmion.CommunicatorCms.Core.Application.Pages
{
    public class AppPage
    {
        public static AppPage EmptyAppPage { get; } = new AppPage("/");
        private static AppPageProperties AppPagePropertiesDefault { get; } = new AppPageProperties();

        private static char IgnoreContentFileStartingCharacter = '_';
        private static IDeserializer YamlDeserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        private static MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder().UseAutoIdentifiers().Build();

        public string PageUrl { get; set; }
        public string PageAppPath { get; set; }
        public string[] ParameterKeys { get; }

        public bool HasParameters => ParameterKeys.Length > 0;

        public AppPageProperties Properties { get; set; } = new AppPageProperties();
        public AppPageProperties PropertiesMaster { get; set; } = new AppPageProperties();
        public AppPagePropertiesNavItem PropertiesNavItem { get; set; } = new AppPagePropertiesNavItem();
        public dynamic PropertiesExtra { get; set; } = 0;

        public List<string> ContentFileAppPaths => GetContentFileAppPaths();

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

            if (baseUrl == "/") 
            {
                return new AppPageBaseUrlAndParameterValues(url, new string[0]);
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

        public async Task Reload(bool reloadSubPages = true)
        {
            _subPages = null;
            _contentFileAppPaths = null;

            await LoadProperties();

            if (reloadSubPages && Properties != AppPagePropertiesDefault) 
            {
                await ReloadSubPagePropertiesRecursively();
            }
        }
        public async Task ReloadSubPagePropertiesRecursively() 
        {
            foreach (var sp in await GetSubPages())
            {
                await sp.LoadProperties();
                await sp.ReloadSubPagePropertiesRecursively();
            }
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

            for (var i = c - 1; i >= 0; i--) 
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
            // TODO: Split into sepearate Try Catch.
            // TODO: Maybe remove InheritanceUrl, bc, now InheritanceUrl is checked from uniherited Properties, thus InheritanceUrl is not inherited.
            try
            {
                var properties = await LoadPropertiesFromUrl<AppPageProperties>(PageUrl, AppPageSettings.PropertiesFileName);
                var propertiesMaster = await LoadPropertiesFromUrl<AppPageProperties>(PageUrl, AppPageSettings.PropertiesMasterFileName);
                var propertiesNavItem = await LoadPropertiesFromUrl<AppPagePropertiesNavItem>(PageUrl, AppPageSettings.PropertiesNavItemFileName);
                var propertiesExtra = await LoadPropertiesFromUrl<AppPagePropertiesExtra>(PageUrl, AppPageSettings.PropertiesExtraFileName);

                Properties = properties;
                PropertiesMaster = propertiesMaster;
                PropertiesNavItem = propertiesNavItem;
                PropertiesExtra = propertiesExtra;

                // Inheritance
                var realInheritanceRootUrl = properties.InheritanceRootUrl;
                if (properties.InheritanceRootUrl == "")
                {
                    // Inherit self
                    realInheritanceRootUrl = PageUrl;
                }
                else if (properties.InheritanceRootUrl.StartsWith('.'))
                {
                    // Covers both .. and .
                    // TODO: FIX
                    realInheritanceRootUrl = AppPath.ConvertAbsolutePathToAppUrl(Path.GetFullPath(AppUrl.ConvertToAbsolutePath(PageUrl) + "/" + properties.InheritanceRootUrl));
                }

                if (PageUrl.StartsWith(realInheritanceRootUrl)) 
                {
                    // Currently this loads all YAML files.
                    // Maybe better to use cached pages??


                    // Off by 1, but does not matter, since those chars still == "/", which is being split on
                    var relativePageUrlFromInheritanceRootUrl = PageUrl.Substring(realInheritanceRootUrl.Length, PageUrl.Length - realInheritanceRootUrl.Length);

                    var splitRelativePageUrl = relativePageUrlFromInheritanceRootUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);

                    var currentPropertyDictionary = await LoadPropertiesDictionaryFromUrl(realInheritanceRootUrl, AppPageSettings.PropertiesMasterFileName);

                    var currentUrl = realInheritanceRootUrl;

                    foreach (var split in splitRelativePageUrl)
                    {
                        currentUrl += "/" + split;

                        currentPropertyDictionary = MergePropertyDictionaries(currentPropertyDictionary, await LoadPropertiesDictionaryFromUrl(currentUrl, AppPageSettings.PropertiesMasterFileName));
                    }

                    // Override all inherited properties with those in the main properties file.
                    currentPropertyDictionary = MergePropertyDictionaries(currentPropertyDictionary, await LoadPropertiesDictionaryFromUrl(PageUrl, AppPageSettings.PropertiesFileName));

                    // TODO: FIX
                    // UGLY AF
                    // MAYBE SWITCH TO REFLECTION??
                    var serialized = App.YamlSerializer.Serialize(currentPropertyDictionary);
                    Properties = App.YamlDeserializer.Deserialize<AppPageProperties>(serialized);

                    /*
                    DOES NOT WORK!!
                    foreach (var property in AppPageProperties.WriteableProperties)
                    {
                        if (currentPropertyDictionary.TryGetValue(property.Name, out var value))
                        {
                            property.SetValue(Properties, Convert.ChangeType(value, property.PropertyType));
                        }
                    }
                    */

                }
            }
            catch (Exception e)
            {

            }
        }

        private Dictionary<object, object> MergePropertyDictionaries(Dictionary<object, object> basePropertyDictionary, Dictionary<object, object> overridingPropertyDictionary) 
        {
            foreach (var kvp in overridingPropertyDictionary) 
            {
                basePropertyDictionary[kvp.Key] = kvp.Value;
            }

            return basePropertyDictionary;
        }

        private async Task<T> LoadPropertiesFromUrl<T>(string url, string propertiesFileName) where T : new()
        {
            var propertiesFileUrl = AppUrl.Join(url, propertiesFileName);

            if (AppUrl.Exists(propertiesFileUrl))
            {
                var propertiesCandidate = YamlDeserializer.Deserialize<T>(await AppUrl.ReadAllTextAsync(propertiesFileUrl));
                if (propertiesCandidate != null)
                {
                    return propertiesCandidate;
                }
            }

            return new T();
        }

        private async Task<Dictionary<object, object>> LoadPropertiesDictionaryFromUrl(string url, string propertiesFileName) 
        {
            var propertiesFileUrl = AppPath.Join(url, propertiesFileName);

            if (AppUrl.Exists(propertiesFileUrl))
            {
                var propertiesCandidate = YamlDeserializer.Deserialize<Dictionary<object, object>>(await AppUrl.ReadAllTextAsync(propertiesFileUrl));
                if (propertiesCandidate != null)
                {
                    return propertiesCandidate;
                }
            }

            return new Dictionary<object, object>();
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
