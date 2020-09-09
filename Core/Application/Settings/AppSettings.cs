using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCmsLibrary.Core.Settings
{
    public class AppSettings
    {
        public string Title { get; set; } = "";
        public string Theme { get; set; } = ThemeSettings.DefaultName;

        public string LogoIcon { get; set; } = "";
        public string LogoContent { get; set; } = "";

        public string Version { get; set; } = "";

        public int LinuxLocalHostPort { get; set; } = 5000;

        public string[] RootPageUrls { get; set; } = new string[] { "/" };
    }
}
