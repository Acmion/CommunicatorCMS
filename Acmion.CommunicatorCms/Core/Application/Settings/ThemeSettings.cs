using Acmion.CommunicatorCms.Core.Application.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Settings
{
    public class ThemeSettings
    {
        public static string CoreDirectoryName { get; } = "core";
        public static string LayoutFileName { get; } = "layout.cshtml";

        public string HeaderClass { get; set; } = "";
        public string FooterClass { get; set; } = "";
        public string MenuTopClass { get; set; } = "";

        public ThemeCenterLayout CenterLayout { get; set; } = new ThemeCenterLayout();
    }
}
