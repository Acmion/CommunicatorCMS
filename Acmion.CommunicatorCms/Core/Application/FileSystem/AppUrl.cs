using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Acmion.CommunicatorCms.Core.Extensions;
using Acmion.CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.WebUtilities;

namespace Acmion.CommunicatorCms.Core.Application.FileSystem
{
    public static class AppUrl
    {
        public static char Separator = '/';
        public static string SeparatorString = "/";
        public static string[] UnservableContainsStrings = { "/_", "/.git" };
        public static string[] UnservableEndsWithStrings = { ".cshtml" };

        public static string ConvertAspNetCoreUrlToActualUrl(string url)
        {
            if (url.StartsWith("~/"))
            {
                url = url.ReplaceFirst("~/", "/");
            }

            if (url.StartsWith(GeneralSettings.RazorPagesRootAppPath)) 
            {
                url = url.ReplaceFirst(GeneralSettings.RazorPagesRootAppPath, "");
            }

            if (url.EndsWith(AppPageSettings.IndexUrl)) 
            {
                url = url.Substring(0, url.Length - AppPageSettings.IndexUrl.Length);
            }

            var indexOfWwwRoot = url.IndexOf(GeneralSettings.WwwRootAppPath);
            if (indexOfWwwRoot >= 0) 
            {
                url = url.Substring(indexOfWwwRoot + GeneralSettings.WwwRootAppPath.Length, url.Length - indexOfWwwRoot - GeneralSettings.WwwRootAppPath.Length);
            }

            return url;
        }

        public static string ConvertToAppPath(string url)
        {
            return AppPath.Join(GeneralSettings.RazorPagesRootAppPath, url);
        }
        public static string ConvertToAbsolutePath(string url)
        {
            return AppPath.Join(CommunicatorCmsConfiguration.AppAbsolutePath, ConvertToAppPath(url));
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
            foreach (var unservableString in UnservableEndsWithStrings)
            {
                if (url.EndsWith(unservableString))
                {
                    return false;
                }
            }

            foreach (var unservableString in UnservableContainsStrings)
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
                dirs[i] = AppPath.ConvertToAppUrl(dirs[i]);    
            }

            return dirs;
        }
        public static string[] GetFiles(string url, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var files = AppDirectory.GetFiles(ConvertToAppPath(url), searchPattern, searchOption);

            for (var i = 0; i < files.Length; i++)
            {
                files[i] = AppPath.ConvertToAppUrl(files[i]);
            }

            return files;
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

        public static string ParentUrl(string url) 
        {
            var lastSeparatorIndex = url.LastIndexOf(Separator, url.Length - 1 - 1, url.Length - 1);
            return url.Substring(0, lastSeparatorIndex + 1);
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
