using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCmsLibrary.Core.Application.FileSystem
{
    public static class AppFile
    {
        public static bool Exists(string appPath) 
        {
            var a = AppPath.ConvertToAbsolutePath(appPath);
            return File.Exists(AppPath.ConvertToAbsolutePath(appPath));
        }

        public static string ReadAllText(string appPath)
        {
            return File.ReadAllText(AppPath.ConvertToAbsolutePath(appPath));
        }

        public static string[] ReadAllLines(string appPath)
        {
            return File.ReadAllLines(AppPath.ConvertToAbsolutePath(appPath));
        }

        public static async Task<string> ReadAllTextAsync(string appPath)
        {
            return await File.ReadAllTextAsync(AppPath.ConvertToAbsolutePath(appPath));
        }

        public static async Task<string[]> ReadAllLinesAsync(string appPath)
        {
            return await File.ReadAllLinesAsync(AppPath.ConvertToAbsolutePath(appPath));
        }

    }
}
