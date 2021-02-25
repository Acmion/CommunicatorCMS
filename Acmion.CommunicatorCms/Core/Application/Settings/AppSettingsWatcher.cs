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
    public class AppSettingsWatcher
    {
        public bool FileChanged { get; set; }
        public bool WatcherFailed { get; set; }

        public FileSystemWatcher SettingsFileWatcher { get; }

        public AppSettingsWatcher()
        {
            var appSettingsFilePath = AppUrl.Join(UrlSettings.ContentSecondClassUrl, "_settings.yaml");

            SettingsFileWatcher = CreatePageFileWatcher(appSettingsFilePath, Changed, Deleted, Renamed, WatcherError);
        }

        private FileSystemWatcher CreatePageFileWatcher(string url, FileSystemEventHandler onChanged, FileSystemEventHandler onDeleted, RenamedEventHandler onRenamed, ErrorEventHandler onError)
        {
            var propertiesWatcher = new FileSystemWatcher();

            var path = AppUrl.ConvertToAbsolutePath(url);
            propertiesWatcher.Path = AppPath.GetDirectoryName(path);
            propertiesWatcher.Filter = AppPath.GetFileName(path);

            propertiesWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            propertiesWatcher.IncludeSubdirectories = false;

            propertiesWatcher.Changed += onChanged;
            propertiesWatcher.Created += onChanged;
            propertiesWatcher.Deleted += onDeleted;
            propertiesWatcher.Renamed += onRenamed;
            propertiesWatcher.Error += onError;

            propertiesWatcher.EnableRaisingEvents = true;

            return propertiesWatcher;
        }

        private void Deleted(object sender, FileSystemEventArgs e)
        {
            FileChanged = true;
        }

        private void Renamed(object sender, RenamedEventArgs e)
        {
            FileChanged = true;
        }

        private void Changed(object sender, FileSystemEventArgs e)
        {
            FileChanged = true;
        }

        private void WatcherError(object sender, ErrorEventArgs e)
        {
            WatcherFailed = true;
        }
    }
}
