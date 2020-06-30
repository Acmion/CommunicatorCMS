using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunicatorCms.Core.Settings
{
    public static class AppSettings
    {
        public static string Logo { get; set; } = "";
        public static string Title { get; set; } = "";
        public static string Version { get; set; } = "";

        public static int LinuxLocalHostPort { get; set; } = 5000;
    }
}
