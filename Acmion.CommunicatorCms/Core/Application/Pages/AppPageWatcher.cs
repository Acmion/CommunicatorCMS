using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application.FileSystem;
using Acmion.CommunicatorCms.Core.Settings;

namespace Acmion.CommunicatorCms.Core.Application.Pages
{

    public class AppPageWatcher
    {
        public bool PageWatcherFailed { get; set; }

        public ConcurrentDictionary<string, bool> ChangedPageUrls { get; } = new ConcurrentDictionary<string, bool>();

        public FileSystemWatcher PageWatcher { get; }
        public FileSystemWatcher PageFileWatcher { get; }

        public AppPageWatcher()
        {
            PageWatcher = CreatePageWatcher(UrlSettings.ContentRootUrl, PageChanged, PageDeleted, PageRenamed, PageWatcherError);
            PageFileWatcher = CreatePageFileWatcher(UrlSettings.ContentRootUrl, PageChanged, PageDeleted, PageRenamed, PageWatcherError);
        }

        private FileSystemWatcher CreatePageWatcher(string url, FileSystemEventHandler onChanged, FileSystemEventHandler onDeleted, RenamedEventHandler onRenamed, ErrorEventHandler onError)
        {
            var watcher = new FileSystemWatcher();

            watcher.Path = AppUrl.ConvertToAbsolutePath(url);

            watcher.NotifyFilter = NotifyFilters.DirectoryName;
            watcher.IncludeSubdirectories = true;

            watcher.Changed += onChanged;
            watcher.Created += onChanged;
            watcher.Deleted += onDeleted;
            watcher.Renamed += onRenamed;
            watcher.Error += onError;

            watcher.EnableRaisingEvents = true;

            return watcher;
        }
        private FileSystemWatcher CreatePageFileWatcher(string url, FileSystemEventHandler onChanged, FileSystemEventHandler onDeleted, RenamedEventHandler onRenamed, ErrorEventHandler onError)
        {
            var propertiesWatcher = new FileSystemWatcher();

            propertiesWatcher.Path = AppUrl.ConvertToAbsolutePath(url);

            propertiesWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            propertiesWatcher.IncludeSubdirectories = true;

            propertiesWatcher.Changed += onChanged;
            propertiesWatcher.Created += onChanged;
            propertiesWatcher.Deleted += onDeleted;
            propertiesWatcher.Renamed += onRenamed;
            propertiesWatcher.Error += onError;

            propertiesWatcher.EnableRaisingEvents = true;

            return propertiesWatcher;
        }

        private void PageDeleted(object sender, FileSystemEventArgs e)
        {
            AddChangedPage(e.FullPath, sender, ChangedPageUrls);
        }

        private void PageRenamed(object sender, RenamedEventArgs e)
        {
            AddChangedPage(e.OldFullPath, sender, ChangedPageUrls);
        }

        private void PageChanged(object sender, FileSystemEventArgs e)
        {
            AddChangedPage(e.FullPath, sender, ChangedPageUrls);
        }

        private void PageWatcherError(object sender, ErrorEventArgs e)
        {
            PageWatcherFailed = true;
        }

        private void AddChangedPage(string fullPath, object sender, ConcurrentDictionary<string, bool> changedPageUrls)
        {
            var replaced = fullPath.Replace('\\', '/');
            var pageUrl = AppPath.ConvertAbsolutePathToAppUrl(replaced);

            if (!AppPage.IsUrlAppPage(pageUrl))
            {
                pageUrl = AppPath.GetDirectoryName(pageUrl);
            }

            if (!changedPageUrls.ContainsKey(pageUrl) && AppPage.IsUrlAppPage(pageUrl)) 
            {
                changedPageUrls[pageUrl] = true;
            }
        }
    }
}
