﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application.Extensions;
using Acmion.CommunicatorCms.Core.Application.FileSystem;
using Acmion.CommunicatorCms.Core.Application.Pages;
using Acmion.CommunicatorCms.Core.Application.UrlRewrite;
using Acmion.CommunicatorCms.Core.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using YamlDotNet.Serialization;

namespace Acmion.CommunicatorCms.Core.Application
{
    public static class App
    {
        public static AppPageHandler Pages => PageHandler;
        public static dynamic Extensions => ExtensionHandler.Extensions;

        public static AppSettings Settings { get; private set; } = new AppSettings();
        public static ThemeSettings ThemeSettings { get; private set; } = new ThemeSettings();

        public static AppPageHandler PageHandler { get; } = new AppPageHandler();
        public static AppPageWatcher PageWatcher { get; } = new AppPageWatcher();

        public static AppExtensionHandler ExtensionHandler { get; } = new AppExtensionHandler();
        public static AppExtensionWatcher ExtensionWatcher { get; } = new AppExtensionWatcher();

        public static IUrlRewriter UrlRewriteCustom { get; set; } = new UrlRewriterNone();

        private static bool Launched { get; set; }
        private static SemaphoreSlim PageSemaphoreSlim = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim ExtensionSemaphoreSlim = new SemaphoreSlim(1, 1);

        private static IDeserializer YamlDeserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();

        static App() 
        {
            var appSettingsContent = AppFile.ReadAllText(AppPath.Join(GeneralSettings.RazorPagesRootAppPath, UrlSettings.ContentSecondClassUrl, "_settings.yaml"));
            Settings = YamlDeserializer.Deserialize<AppSettings>(appSettingsContent);

            Settings.Initialize();

            var themeSettingsContent = AppFile.ReadAllText(AppPath.Join(GeneralSettings.RazorPagesRootAppPath, UrlSettings.ContentThirdClassThemesUrl, Settings.Theme, "_settings.yaml"));
            ThemeSettings = YamlDeserializer.Deserialize<ThemeSettings>(themeSettingsContent);
        }

        public static async Task OnRequestStart(IHtmlHelper htmlHelper)
        {
            if (!Launched)
            {
                Launched = true;

                await Launch(htmlHelper);
            }

            await HandleChangedAppPages(htmlHelper);
            await HandleChangedAppExtensions(htmlHelper);
        }

        private static async Task Launch(IHtmlHelper htmlHelper)
        {
            // Only one thread can execute this at a time

            await ExtensionSemaphoreSlim.WaitAsync();

            try
            {
                await ExtensionHandler.LoadAppExtensions(htmlHelper);
            }
            finally
            {
                ExtensionSemaphoreSlim.Release();
            }
        }

        private static async Task HandleChangedAppPages(IHtmlHelper htmlHelper)
        {
            // Just an extra check so that unnecessary semaphore is avoided
            if (PageWatcher.CmsPageWatcherFailed || PageWatcher.ContentPageWatcherFailed ||
                PageWatcher.CmsChangedPageUrls.Count > 0 || PageWatcher.ContentChangedPageUrls.Count > 0)
            {
                await PageSemaphoreSlim.WaitAsync();

                // Only one thread can execute this at a time

                try
                {
                    if (PageWatcher.CmsPageWatcherFailed || PageWatcher.ContentPageWatcherFailed)
                    {
                        PageHandler.AppPagesByUrl.Clear();

                        PageWatcher.CmsPageWatcherFailed = false;
                        PageWatcher.ContentPageWatcherFailed = false;

                        PageWatcher.CmsChangedPageUrls.Clear();
                        PageWatcher.ContentChangedPageUrls.Clear();
                    }

                    var allPageUrls = PageWatcher.ContentChangedPageUrls.Concat(PageWatcher.CmsChangedPageUrls);

                    foreach (var pageUrl in allPageUrls)
                    {
                        if (AppPage.IsUrlAppPage(pageUrl))
                        {
                            var appPage = await Pages.GetByUrl(pageUrl);

                            await appPage.Reload();
                        }
                        else
                        {
                            // Page has been removed or no longer exists
                            Pages.RemoveUrl(pageUrl);
                        }

                        if (pageUrl != AppUrl.SeparatorString && pageUrl != "") 
                        {
                            // Refresh parent page as well
                            var parentUrl = AppPath.GetDirectoryName(pageUrl);

                            if (AppPage.IsUrlAppPage(parentUrl))
                            {
                                var parentPage = await Pages.GetByUrl(parentUrl);

                                await parentPage.Reload();
                            }
                        }
                    }

                    PageWatcher.CmsChangedPageUrls.Clear();
                    PageWatcher.ContentChangedPageUrls.Clear();
                }
                finally
                {
                    PageSemaphoreSlim.Release();
                }
            }

        }
        private static async Task HandleChangedAppExtensions(IHtmlHelper htmlHelper)
        {
            /*
            // Just an extra check so that unnecessary semaphore is avoided
            if (ExtensionWatcher.CmsExtensionWatcherFailed || ExtensionWatcher.Failed ||
                ExtensionWatcher.CmsChangedExtensionUrls.Count > 0 || ExtensionWatcher.ChangedExtensionUrls.Count > 0)
            {
                await ExtensionSemaphoreSlim.WaitAsync();
                
                // Only one thread can execute this at a time

                try
                {
                    if (ExtensionWatcher.CmsExtensionWatcherFailed || ExtensionWatcher.Failed)
                    {
                        await ExtensionHandler.LoadAppExtensions(htmlHelper);

                        ExtensionWatcher.CmsExtensionWatcherFailed = false;
                        ExtensionWatcher.Failed = false;

                        ExtensionWatcher.CmsChangedExtensionUrls.Clear();
                        ExtensionWatcher.ChangedExtensionUrls.Clear();
                    }

                    foreach (var extUrl in ExtensionWatcher.CmsChangedExtensionUrls)
                    {
                        await ExtensionHandler.LoadAppExtension(extUrl, htmlHelper);
                    }

                    foreach (var extUrl in ExtensionWatcher.ChangedExtensionUrls)
                    {
                        await ExtensionHandler.LoadAppExtension(extUrl, htmlHelper);
                    }

                    ExtensionWatcher.CmsChangedExtensionUrls.Clear();
                    ExtensionWatcher.ChangedExtensionUrls.Clear();
                }
                finally
                {
                    ExtensionSemaphoreSlim.Release();
                }
            }
            */

        }


    }
}
