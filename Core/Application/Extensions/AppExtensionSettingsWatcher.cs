using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application.FileSystem;
using Acmion.CommunicatorCms.Core.Settings;

namespace Acmion.CommunicatorCms.Core.Application.Extensions
{
    public class AppExtensionSettingsWatcher
    {
        public bool CmsExtensionSettingsWatcherFailed { get; set; }
        public bool ContentExtensionSettingsWatcherFailed { get; set; }

        public HashSet<string> CmsChangedExtensionSettingsUrls { get; } = new HashSet<string>();
        public HashSet<string> ContentChangedExtensionSettingsUrls { get; } = new HashSet<string>();

        public FileSystemWatcher CmsExtensionSettingsWatcher { get; }
        public FileSystemWatcher ContentExtensionSettingsWatcher { get; }

        public AppExtensionSettingsWatcher() 
        {
            CmsExtensionSettingsWatcher = CreateExtensionWatcher(UrlSettings.CmsCommunicatorCmsExtensionsUrl, CmsExtensionSettingsChanged, CmsExtensionSettingsChanged, CmsExtensionSettingsWatcherError);
            ContentExtensionSettingsWatcher = CreateExtensionWatcher(UrlSettings.ContentCommunicatorCmsExtensionsUrl, ContentExtensionSettingsChanged, ContentExtensionSettingsChanged, ContentExtensionSettingsWatcherError);
        }

        private FileSystemWatcher CreateExtensionWatcher(string url, FileSystemEventHandler onChanged, RenamedEventHandler onRenamed, ErrorEventHandler onError) 
        {
            var watcher = new FileSystemWatcher();

            watcher.Path = AppUrl.ConvertToAbsolutePath(url);

            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.IncludeSubdirectories = true;

            watcher.Changed += onChanged;
            watcher.Created += onChanged;
            watcher.Deleted += onChanged;
            watcher.Renamed += onRenamed;
            watcher.Error += onError;

            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private void CmsExtensionSettingsChanged(object sender, FileSystemEventArgs e)
        {
            AddChangedExtension(e.FullPath, UrlSettings.CmsCommunicatorCmsExtensionsUrl, CmsChangedExtensionSettingsUrls);
        }

        private void ContentExtensionSettingsChanged(object sender, FileSystemEventArgs e)
        {
            AddChangedExtension(e.FullPath, UrlSettings.ContentCommunicatorCmsExtensionsUrl, ContentChangedExtensionSettingsUrls);
        }

        private void CmsExtensionSettingsWatcherError(object sender, ErrorEventArgs e)
        {
            CmsExtensionSettingsWatcherFailed = true;
        }

        private void ContentExtensionSettingsWatcherError(object sender, ErrorEventArgs e)
        {
            ContentExtensionSettingsWatcherFailed = true;
        }

        private void AddChangedExtension(string fullPath, string extensionRootUrl, HashSet<string> changedExtensionUrls) 
        {
            var fullUrl = AppPath.ConvertAbsolutePathToAppUrl(fullPath.Replace('\\', '/'));
            var separatorIndex = fullUrl.IndexOf(AppUrl.Separator, extensionRootUrl.Length + 1);

            var extensionUrl = fullUrl.Substring(0, separatorIndex);

            if (!changedExtensionUrls.Contains(extensionUrl))
            {
                changedExtensionUrls.Add(extensionUrl);
            }
        }
    }
}
