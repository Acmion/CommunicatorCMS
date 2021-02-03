using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Application.Pages.Properties
{
    public class AppPageProperties
    {
        public static PropertyInfo[] WriteableProperties { get; } = typeof(AppPageProperties).GetProperties().Where(p => p.CanWrite).ToArray();

        public string Icon { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string RedirectUrl { get; set; } = "";
        public string InheritanceRootUrl { get; set; } = "/";

        public string Layout { get; set; } = "";
        public AppPageAuthorization Authorization { get; set; } = new AppPageAuthorization();

        public bool RenderMain { get; set; } = true;
        public bool RenderHead { get; set; } = true;
        public bool RenderHeader { get; set; } = true;
        public bool RenderFooter { get; set; } = true;
        public bool RenderTopMenu { get; set; } = true;
        public bool RenderLeftMenu { get; set; } = true;
        public bool RenderRightMenu { get; set; } = true;

        public bool ShowInNavigationMenus { get; set; } = true;
        
        public List<string> SubPageOrder { get; set; } = new List<string>();
        public List<string> SubPageOrderFromEnd { get; set; } = new List<string>();

        public List<string> ContentOrder { get; set; } = new List<string>();
    }

    public class AppPageAuthorization 
    {
        public List<string> Roles { get; set; } = new List<string>();
    }
}
