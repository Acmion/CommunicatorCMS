using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application.FileSystem;
using Acmion.CommunicatorCms.Core.Settings;

namespace Acmion.CommunicatorCms.Core.Application.Extensions
{
    public class AppExtensionWatcher
    {
        public bool CmsExtensionWatcherFailed { get; set; }
        public bool ContentExtensionWatcherFailed { get; set; }

        public HashSet<string> CmsChangedExtensionUrls { get; } = new HashSet<string>();
        public HashSet<string> ContentChangedExtensionUrls { get; } = new HashSet<string>();

        public FileSystemWatcher CmsExtensionWatcher { get; }
        public FileSystemWatcher ContentExtensionWatcher { get; }

        public FileSystemWatcher CmsExtensionSettingsWatcher { get; }
        public FileSystemWatcher ContentExtensionSettingsWatcher { get; }

        public AppExtensionWatcher() 
        {
            CmsExtensionWatcher = CreateExtensionWatcher(UrlSettings.CmsCommunicatorCmsExtensionsUrl, CmsExtensionChanged, CmsExtensionChanged, CmsExtensionWatcherError);
            ContentExtensionWatcher = CreateExtensionWatcher(UrlSettings.ContentCommunicatorCmsExtensionsUrl, ContentExtensionChanged, ContentExtensionChanged, ContentExtensionWatcherError);

            CmsExtensionSettingsWatcher = CreateExtensionWatcher(UrlSettings.CmsGeneralSettingsExtensionsUrl, CmsExtensionChanged, CmsExtensionChanged, CmsExtensionWatcherError);
            ContentExtensionSettingsWatcher = CreateExtensionWatcher(UrlSettings.ContentGeneralSettingsExtensionUrl, ContentExtensionChanged, ContentExtensionChanged, ContentExtensionWatcherError);
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

        private void CmsExtensionChanged(object sender, FileSystemEventArgs e)
        {
            AddChangedExtension(e.FullPath, UrlSettings.CmsCommunicatorCmsExtensionsUrl, CmsChangedExtensionUrls);
        }

        private void ContentExtensionChanged(object sender, FileSystemEventArgs e)
        {
            AddChangedExtension(e.FullPath, UrlSettings.ContentCommunicatorCmsExtensionsUrl, ContentChangedExtensionUrls);
        }

        private void CmsExtensionWatcherError(object sender, ErrorEventArgs e)
        {
            CmsExtensionWatcherFailed = true;
        }

        private void ContentExtensionWatcherError(object sender, ErrorEventArgs e)
        {
            ContentExtensionWatcherFailed = true;
        }

        private void AddChangedExtension(string fullPath, string extensionRootUrl, HashSet<string> changedExtensionUrls) 
        {
            var formattedFullPath = fullPath.Replace('\\', '/')
                                            .Replace(UrlSettings.CmsGeneralSettingsExtensionsUrl, UrlSettings.CmsCommunicatorCmsExtensionsUrl)
                                            .Replace(UrlSettings.ContentGeneralSettingsExtensionUrl, UrlSettings.ContentCommunicatorCmsExtensionsUrl);
            var fullUrl = AppPath.ConvertAbsolutePathToAppUrl(formattedFullPath);
            var separatorIndex = fullUrl.IndexOf(AppUrl.Separator, extensionRootUrl.Length + 1);

            var extensionUrl = fullUrl;

            if (separatorIndex == -1)
            {
                // Extension top directory changed event
            }
            else 
            {
                // Extension file or subdirectory changed
                extensionUrl = fullUrl.Substring(0, separatorIndex);

                if (!changedExtensionUrls.Contains(extensionUrl))
                {
                    changedExtensionUrls.Add(extensionUrl);
                }
            }
        }
    }
}
