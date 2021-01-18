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
        public bool Failed { get; set; }

        public HashSet<string> ChangedExtensionUrls { get; } = new HashSet<string>();

        public FileSystemWatcher ExtensionWatcher { get; }
        public FileSystemWatcher ExtensionSettingsWatcher { get; }

        public AppExtensionWatcher() 
        {
            ExtensionWatcher = CreateExtensionWatcher(UrlSettings.ContentThirdClassExtensionsUrl, ExtensionChanged, ExtensionChanged, ExtensionWatcherError);
            //ExtensionSettingsWatcher = CreateExtensionWatcher(UrlSettings.ContentGeneralSettingsExtensionUrl, ContentExtensionChanged, ContentExtensionChanged, ContentExtensionWatcherError);
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

        private void ExtensionChanged(object sender, FileSystemEventArgs e)
        {
            AddChangedExtension(e.FullPath, UrlSettings.ContentThirdClassExtensionsUrl, ChangedExtensionUrls);
        }
        private void ExtensionWatcherError(object sender, ErrorEventArgs e)
        {
            Failed = true;
        }

        private void AddChangedExtension(string fullPath, string extensionRootUrl, HashSet<string> changedExtensionUrls) 
        {
            var formattedFullPath = fullPath.Replace('\\', '/');
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
