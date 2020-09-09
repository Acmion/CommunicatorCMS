using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCmsLibrary.Core.Settings
{
    public static class ThemeSettings
    {
        public static string DefaultName { get; set; } = "default";

        public static string LayoutFileName { get; } = "_layout.cshtml";

    }
}
