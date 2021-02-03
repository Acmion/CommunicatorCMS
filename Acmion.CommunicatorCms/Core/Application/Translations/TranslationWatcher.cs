using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application.FileSystem;
using Acmion.CommunicatorCms.Core.Settings;

namespace Acmion.CommunicatorCms.Core.Application.Translations
{

    public class TranslationWatcher
    {
        public bool WatcherFailed { get; set; }
        public FileSystemWatcher Watcher { get; }
        public ConcurrentDictionary<string, bool> ChangedFileUrls { get; } = new ConcurrentDictionary<string, bool>();

        public TranslationWatcher()
        {
            Watcher = CreateWatcher(UrlSettings.ContentRootUrl, PageChanged, PageDeleted, PageRenamed, PageWatcherError);
        }

        private FileSystemWatcher CreateWatcher(string url, FileSystemEventHandler onChanged, FileSystemEventHandler onDeleted, RenamedEventHandler onRenamed, ErrorEventHandler onError)
        {
            var propertiesWatcher = new FileSystemWatcher();

            propertiesWatcher.Path = AppUrl.ConvertToAbsolutePath(url);

            propertiesWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.FileName;
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
            AddChangedPage(e.FullPath, sender);
        }

        private void PageRenamed(object sender, RenamedEventArgs e)
        {
            AddChangedPage(e.OldFullPath, sender);
        }

        private void PageChanged(object sender, FileSystemEventArgs e)
        {
            AddChangedPage(e.FullPath, sender);
        }

        private void PageWatcherError(object sender, ErrorEventArgs e)
        {
            WatcherFailed = true;
        }

        private void AddChangedPage(string fullPath, object sender)
        {
            var replaced = fullPath.Replace('\\', '/');
            var url = AppPath.ConvertAbsolutePathToAppUrl(replaced);

            if (!ChangedFileUrls.ContainsKey(url)) 
            {
                ChangedFileUrls[url] = true;
            }
        }
    }
}
