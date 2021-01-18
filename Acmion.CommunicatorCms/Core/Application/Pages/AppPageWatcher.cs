using System;
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
        public bool CmsPageWatcherFailed { get; set; }
        public bool ContentPageWatcherFailed { get; set; }

        public HashSet<string> CmsChangedPageUrls { get; } = new HashSet<string>();
        public HashSet<string> ContentChangedPageUrls { get; } = new HashSet<string>();

        public FileSystemWatcher CmsPageWatcher { get; }
        public FileSystemWatcher ContentPageWatcher { get; }

        public FileSystemWatcher CmsPageFileWatcher { get; }
        public FileSystemWatcher ContentPageFileWatcher { get; }

        public AppPageWatcher()
        {
            CmsPageWatcher = CreatePageWatcher(UrlSettings.CmsRootUrl, CmsPageChanged, CmsPageDeleted, CmsPageRenamed, CmsPageWatcherError);
            ContentPageWatcher = CreatePageWatcher(UrlSettings.ContentRootUrl, ContentPageChanged, ContentPageDeleted, ContentPageRenamed, ContentPageWatcherError);

            CmsPageFileWatcher = CreatePageFileWatcher(UrlSettings.CmsRootUrl, CmsPageChanged, CmsPageDeleted, CmsPageRenamed, CmsPageWatcherError);
            ContentPageFileWatcher = CreatePageFileWatcher(UrlSettings.ContentRootUrl, ContentPageChanged, ContentPageDeleted, ContentPageRenamed, ContentPageWatcherError);
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

        private void CmsPageDeleted(object sender, FileSystemEventArgs e)
        {
            AddChangedPage(e.FullPath, sender, CmsChangedPageUrls);
        }
        private void ContentPageDeleted(object sender, FileSystemEventArgs e)
        {
            AddChangedPage(e.FullPath, sender, ContentChangedPageUrls);
        }

        private void CmsPageRenamed(object sender, RenamedEventArgs e)
        {
            AddChangedPage(e.OldFullPath, sender, CmsChangedPageUrls);
        }
        private void ContentPageRenamed(object sender, RenamedEventArgs e)
        {
            AddChangedPage(e.OldFullPath, sender, ContentChangedPageUrls);
        }

        private void CmsPageChanged(object sender, FileSystemEventArgs e)
        {
            AddChangedPage(e.FullPath, sender, CmsChangedPageUrls);
        }
        private void ContentPageChanged(object sender, FileSystemEventArgs e)
        {
            AddChangedPage(e.FullPath, sender, ContentChangedPageUrls);
        }

        private void CmsPageWatcherError(object sender, ErrorEventArgs e)
        {
            CmsPageWatcherFailed = true;
        }
        private void ContentPageWatcherError(object sender, ErrorEventArgs e)
        {
            ContentPageWatcherFailed = true;
        }

        private void AddChangedPage(string fullPath, object sender, HashSet<string> changedPageUrls)
        {
            var replaced = fullPath.Replace('\\', '/');
            var pageUrl = AppPath.ConvertAbsolutePathToAppUrl(replaced);

            var directoryEvent = sender == CmsPageWatcher || sender == ContentPageWatcher;

            if (!AppPage.IsUrlAppPage(pageUrl))
            {
                pageUrl = AppPath.GetDirectoryName(pageUrl);
            }

            if (!changedPageUrls.Contains(pageUrl) && AppPage.IsUrlAppPage(pageUrl)) 
            {
                changedPageUrls.Add(pageUrl);
            }
        }
    }
}
