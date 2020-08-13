using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CommunicatorCms.Core.Extensions;
using CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.WebUtilities;

namespace CommunicatorCms.Core.Application.FileSystem
{
    public static class AppUrl
    {
        public static char Separator = '/';
        public static string SeparatorString = "/";
        public static string[] UnservableStrings = { "/_", "/.", ".cshtml" };

        public static string ConvertToActualUrl(string url)
        {
            url = ConvertAspNetCoreUrlToStandardUrl(url);
            var rootUrl = RootUrl(url);

            if (UrlSettings.NonVirtualRootUrls.Contains(rootUrl))
            {
                return url;
            }

            if (url.StartsWith(UrlSettings.ContentRootUrl))
            {
                var rootUrlPlusOne = RootUrlPlusOne(url);
                if (UrlSettings.NonVirtualContentUrls.Contains(rootUrlPlusOne))
                {
                    return url;
                }
                else if (rootUrlPlusOne != UrlSettings.ContentWwwUrl)
                {
                    return url.ReplaceFirst(UrlSettings.ContentRootUrl, UrlSettings.ContentWwwUrl);
                }
            }
            else
            {
                var contentUrl = Join(UrlSettings.ContentRootUrl, rootUrl);

                if (UrlSettings.NonVirtualContentUrls.Contains(contentUrl))
                {
                    return UrlSettings.ContentRootUrl + url;
                }

                return UrlSettings.ContentWwwUrl + url;
            }

            if (url == "")
            {
                return SeparatorString;
            }

            return url;
        }
        public static string ConvertToVirtualUrl(string url)
        {
            url = ConvertAspNetCoreUrlToStandardUrl(url);

            if (url.StartsWith(UrlSettings.ContentWwwUrl))
            {
                url = url.ReplaceFirst(UrlSettings.ContentWwwUrl, "");
            }

            if (url.StartsWith(UrlSettings.ContentRootUrl))
            {
                url = url.ReplaceFirst(UrlSettings.ContentRootUrl, "");
            }

            if (url == "")
            {
                return SeparatorString;
            }

            return url;
        }
        public static string ConvertAspNetCoreUrlToStandardUrl(string url)
        {
            if (url.StartsWith("~/"))
            {
                url = url.ReplaceFirst("~/", "/");
            }

            if (url.StartsWith("/Web"))
            {
                url = url.ReplaceFirst("/Web", "");
            }

            return url;
        }

        public static string ConvertToAppPath(string url)
        {
            return AppPath.Join(GeneralSettings.WebRootPath, ConvertToActualUrl(url));
        }
        public static string ConvertToAbsolutePath(string url)
        {
            return AppPath.Join(Program.RootPath, ConvertToAppPath(url));
        }

        public static string ConvertAppPathToUrl(string appPath)
        {
            return ConvertToVirtualUrl(appPath);
        }
        public static string ConvertAbsoluteAppPathToUrl(string absAppPath)
        {
            return ConvertToVirtualUrl(AppPath.ConvertAbsolutePathToAppPath(absAppPath));
        }

        public static string ConvertAppPathToActualUrl(string appPath)
        {
            return ConvertToActualUrl(appPath);
        }
        public static string ConvertAbsoluteAppPathToActualUrl(string absAppPath)
        {
            return ConvertToActualUrl(AppPath.ConvertAbsolutePathToAppPath(absAppPath));
        }

        public static string CreateGetUrl(string url, Dictionary<string, string> getParameters)
        {
            var getString = "";

            foreach (var kv in getParameters)
            {
                getString += kv.Key + "=" + kv.Value + "&";
            }

            return url + "?" + getString;
        }
        public static string CreateGetUrl(string url, params (string Key, string Value)[] getParameters)
        {
            var getString = "";

            foreach (var kv in getParameters)
            {
                getString += kv.Key + "=" + kv.Value + "&";
            }

            return url + "?" + getString;
        }

        public static string CreateUrlWithQueryPaths(string url, (string QueryPath, string QueryTitle)[] queryPathsAndTitles, params (string Key, string Value)[] queryParameters) 
        {
            var httpValueCollection = HttpUtility.ParseQueryString(string.Empty);

            foreach (var qpt in queryPathsAndTitles)
            {
                httpValueCollection.Add(QuerySettings.PathParameter, qpt.QueryPath);
                httpValueCollection.Add(QuerySettings.TitleParameter, qpt.QueryTitle);
            }

            foreach (var kv in queryParameters)
            {
                httpValueCollection.Add(kv.Key, kv.Value);
            }

            return url + "?" + httpValueCollection.ToString();
        }

        public static string CreateQueryWithQueryPathsAndTitles(string queryString, params (string QueryPath, string QueryTitle)[] queryPathsAndTitles)
        {
            var httpValueCollection = HttpUtility.ParseQueryString(queryString);

            foreach (var qpt in queryPathsAndTitles)
            {
                httpValueCollection.Add(QuerySettings.PathParameter, qpt.QueryPath);
                httpValueCollection.Add(QuerySettings.TitleParameter, qpt.QueryTitle);
            }

            return "?" + httpValueCollection.ToString();
        }

        public static bool Exists(string url)
        {
            var appPath = ConvertToAppPath(url);
            return AppFile.Exists(appPath) || AppDirectory.Exists(appPath);
        }
        public static bool IsFile(string url)
        {
            return AppFile.Exists(ConvertToAppPath(url));
        }
        public static bool IsDirectory(string url)
        {
            return AppDirectory.Exists(ConvertToAppPath(url));
        }
        public static bool IsDirectlyServable(string url)
        {
            foreach (var unservableString in UnservableStrings)
            {
                if (url.Contains(unservableString))
                {
                    return false;
                }
            }

            return true;
        }

        public static string[] GetDirectories(string url) 
        {
            var dirs = AppDirectory.GetDirectories(ConvertToAppPath(url));
            
            var c = dirs.Length;

            for(var i = 0; i < c; i++) 
            {
                dirs[i] = ConvertAppPathToUrl(dirs[i]);    
            }

            return dirs;
        }
        public static string[] GetFiles(string url, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return AppDirectory.GetFiles(ConvertToAppPath(url), searchPattern, searchOption);
        }

        public static async Task<string> ReadAllTextAsync(string url) 
        {
            return await AppFile.ReadAllTextAsync(ConvertToAppPath(url));
        }
        public static async Task<string[]> ReadAllLinesAsync(string url) 
        {
            return await AppFile.ReadAllLinesAsync(ConvertToAppPath(url));

        }

        public static string RootUrl(string url)
        {
            // 2 bc url format: /somtehting/asd => /something/ has 2 separators
            return PartialUrl(url, 2);
        }
        public static string RootUrlPlusOne(string url) 
        {
            return PartialUrl(url, 3);
        }
        public static string PartialUrl(string url, int separatorCount) 
        {
            var separatorsFound = 0;
            var previousSeparatorIndex = -1;

            while (true) 
            {
                var separatorIndex = url.IndexOf(Separator, previousSeparatorIndex + 1);

                if (separatorIndex >= 0)
                {
                    separatorsFound++;
                    previousSeparatorIndex = separatorIndex;
                    if (separatorsFound == separatorCount) 
                    {
                        return url.Substring(0, separatorIndex);
                    }
                }
                else 
                {
                    return url;
                }
            }
        }
        public static List<string> AllPartialUrls(string url) 
        {
            var current = "";
            var split = url.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            var allPartialUrls = new List<string>(split.Length);

            foreach (var part in split) 
            {
                current += part;
                allPartialUrls.Add(current);
            }

            return allPartialUrls;
        }

        public static string Join(params string[] urls) 
        {
            return string.Join(Separator, urls).Replace("//", SeparatorString);
        }
        public static string Join(IEnumerable<string> urls)
        {
            return string.Join(Separator, urls).Replace("//", SeparatorString);
        }

        public static string JoinWithTrailingSlash(params string[] urls) 
        {
            var joined = Join(urls);

            if (joined.EndsWith(Separator)) 
            {
                return joined;
            }
            return joined + Separator;
        }
        public static string JoinWithTrailingSlash(IEnumerable<string> urls)
        {
            var joined = Join(urls);

            if (joined.EndsWith(Separator))
            {
                return joined;
            }
            return joined + Separator;
        }

    }
}
