using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Application.Themes
{
    public class ThemeCenterLayout
    {
        public ThemeCenterLayoutSettings Main { get; set; } = new ThemeCenterLayoutSettings();
        public ThemeCenterLayoutSettings MainAndLeft { get; set; } = new ThemeCenterLayoutSettings();
        public ThemeCenterLayoutSettings MainAndRight { get; set; } = new ThemeCenterLayoutSettings();
        public ThemeCenterLayoutSettings MainAndLeftAndRight { get; set; } = new ThemeCenterLayoutSettings();
    }
}
