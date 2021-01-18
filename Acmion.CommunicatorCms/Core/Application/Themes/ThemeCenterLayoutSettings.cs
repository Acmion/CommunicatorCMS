using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Application.Themes
{
    public class ThemeCenterLayoutSettings
    {
        public string CenterClass { get; set; } = "";

        public string MainClass { get; set; } = "";
        public string MainWrapperClass { get; set; } = "";

        public string MenuLeftClass { get; set; } = "";
        public string MenuLeftWrapperClass { get; set; } = "";

        public string MenuRightClass { get; set; } = "";
        public string MenuRightWrapperClass { get; set; } = "";
    }
}
