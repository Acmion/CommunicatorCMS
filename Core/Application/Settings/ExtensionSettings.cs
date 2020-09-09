using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCmsLibrary.Core.Application.FileSystem;

namespace Acmion.CommunicatorCmsLibrary.Core.Application.Settings
{
    public static class ExtensionSettings
    {

        public static string CoreDirectoryName = "core";

        public static string InitializeFileName = "_initialize.cshtml";

        public static string InitializeFileRelativePath = AppPath.Join(CoreDirectoryName, InitializeFileName);
    }
}
