﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application.FileSystem;

namespace Acmion.CommunicatorCms.Core.Application.Pages
{
    public class AppPageHandler
    {
        public ConcurrentDictionary<string, AppPage> AppPagesByUrl { get; } = new ConcurrentDictionary<string, AppPage>();
        public ConcurrentDictionary<string, AppPage> AppPagesByActualUrl { get; } = new ConcurrentDictionary<string, AppPage>();

        public async Task<AppPage> GetByUrl(string url)
        {
            // Fix concurrent access (with semaphore?)!! In all handlers.

            if (AppPagesByUrl.ContainsKey(url))
            {
                return AppPagesByUrl[url];
            }
            else if (AppPage.IsUrlAppPage(url))
            {
                if (url.Contains("//"))
                {
                    throw new Exception("The url can not contain several consecutive forward slashes!");
                }

                var sourcePage = await AppPage.LoadFromUrl(url);

                AppPagesByUrl[url] = sourcePage;
                AppPagesByUrl[sourcePage.PageUrl] = sourcePage;

                return AppPagesByUrl[url];
            }
            else 
            {
                var actualUrl = AppPage.GetActualAppPageUrl(url);

                if (!AppPagesByActualUrl.ContainsKey(actualUrl))
                {
                    AppPagesByActualUrl[actualUrl] = await AppPage.LoadFromUrl(actualUrl);
                }

                return AppPagesByActualUrl[actualUrl];
            }
        }
        public async Task<AppPage> GetByAppPath(string appPath)
        {
            var url = AppPath.ConvertToAppUrl(appPath);

            return await GetByUrl(url);
        }

        public void Clear() 
        {
            AppPagesByUrl.Clear();
            AppPagesByActualUrl.Clear();
        }
        public void RemoveUrl(string url) 
        {
            // Remove all instances of a url, regardless of whether it ends with "/" or not

            var fixedUrl = url + AppUrl.SeparatorString;

            if (url.EndsWith(AppUrl.Separator))
            {
                fixedUrl = url.Remove(url.Length - 1);
            }

            AppPagesByUrl.Remove(url, out var appPage0);
            AppPagesByUrl.Remove(fixedUrl, out var appPage1);
                                            
            var actualUrl = AppPage.GetActualAppPageUrl(url);
            var fixedActualUrl = AppPage.GetActualAppPageUrl(actualUrl);

            AppPagesByActualUrl.Remove(actualUrl, out var appPage2);
            AppPagesByActualUrl.Remove(fixedActualUrl, out var appPage3);
        }
    }
}
