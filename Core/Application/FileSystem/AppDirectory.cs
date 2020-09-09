using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCmsLibrary.Core.Application.FileSystem
{
    public static class AppDirectory
    {
        public static bool Exists(string applicationPath) 
        {
            return Directory.Exists(AppPath.ConvertToAbsolutePath(applicationPath));
        }

        public static string[] GetFiles(string applicationPath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly) 
        {
            var paths = Directory.GetFiles(AppPath.ConvertToAbsolutePath(applicationPath), searchPattern, searchOption);

            for(var i = 0; i < paths.Length; i++) 
            {
                paths[i] = AppPath.ConvertAbsolutePathToAppPath(paths[i].Replace('\\', '/'));
            }

            return paths;
        }

        public static string[] GetDirectories(string applicationPath) 
        {
            var paths = Directory.GetDirectories(AppPath.ConvertToAbsolutePath(applicationPath));

            for (var i = 0; i < paths.Length; i++)
            {
                paths[i] = AppPath.ConvertAbsolutePathToAppPath(paths[i].Replace('\\', '/'));
            }

            return paths;
        }
    }
}
