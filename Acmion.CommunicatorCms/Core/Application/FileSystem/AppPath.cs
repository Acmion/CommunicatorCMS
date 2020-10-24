using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Settings;

namespace Acmion.CommunicatorCms.Core.Application.FileSystem
{
    public static class AppPath
    {
        public const string DirectorySeparator = "/";

        public static bool IsCmsAppPath(string appPath)
        {
            return appPath.StartsWith(GeneralSettings.RazorPagesRootAppPath + UrlSettings.CmsRootUrl);
        }
        public static bool IsCmsAbsolutePath(string absolutePath) 
        {
            return absolutePath.StartsWith(CommunicatorCmsConfiguration.CmsAbsolutePath);
        }


        public static string ConvertToAppUrl(string appPath) 
        {
            if (IsCmsAppPath(appPath)) 
            {
                return appPath.Replace(GeneralSettings.RazorPagesRootAppPath, "");
            }

            return appPath.Replace(GeneralSettings.RazorPagesRootAppPath, "");

        }
        public static string ConvertToAbsolutePath(string appPath)
        {
            if (IsCmsAppPath(appPath)) 
            {
                return Path.Join(CommunicatorCmsConfiguration.CmsAbsolutePath, appPath);
            }

            return Path.Join(CommunicatorCmsConfiguration.AppAbsolutePath, appPath);
        }

        public static string ConvertAbsolutePathToAppUrl(string absolutePath) 
        {
            if (IsCmsAbsolutePath(absolutePath))
            {
                return ConvertToAppUrl(absolutePath.Replace(CommunicatorCmsConfiguration.CmsAbsolutePath, ""));
            }

            return ConvertToAppUrl(absolutePath.Replace(CommunicatorCmsConfiguration.AppAbsolutePath, ""));
        }
        public static string ConvertAbsolutePathToAppPath(string absolutePath)
        {
            if (IsCmsAbsolutePath(absolutePath)) 
            {
                return absolutePath.Replace(CommunicatorCmsConfiguration.CmsAbsolutePath, "");
            }

            return absolutePath.Replace(CommunicatorCmsConfiguration.AppAbsolutePath, "");
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
        public static string GetDirectoryName(string path)
        {
            var dir = Path.GetDirectoryName(path).Replace('\\', '/');

            if (dir == null) 
            {
                throw new Exception("Error, path has no parent directory.");
            }

            return dir;
        }


        public static string Join(params string[] paths)
        {
            return string.Join('/', paths).Replace("//", "/");
        }

        public static bool IsParentUrl(string parentUrl, string childUrl) 
        {
            if (childUrl.StartsWith(parentUrl)) 
            {
                var parentDirectories = parentUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var childDirectories = childUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);

                var c = parentDirectories.Length;
                for(var i = 0; i < c; i++)
                {
                    if (parentDirectories[i] != childDirectories[i]) 
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
        public static string GetUrlWithMaxNumberOfSlashes(string url, int maxSlashes) 
        {
            var count = 0;
            var c = url.Length;
         
            for(var i = 0; i < c; i++)
            {
                var character = url[i];

                if (character == '/') 
                {
                    count++;
                }

                if (count == maxSlashes) 
                {
                    return url.Substring(0, i);
                }
            }

            return url;
        }
        
    }
}
