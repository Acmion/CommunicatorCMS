using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application.Pages;

namespace Acmion.CommunicatorCms.Core.Application.Utilities
{
    public class Breadcrumb
    {
        public List<BreadcrumbItem> BreadcrumbItems { get; set; }
        public BreadcrumbItem LastBreadcrumbItem => BreadcrumbItems.Last();

        public Breadcrumb() 
        {
            BreadcrumbItems = new List<BreadcrumbItem>();
        }
        public Breadcrumb(List<BreadcrumbItem> breadcrumbItems)
        {
            BreadcrumbItems = breadcrumbItems;
        }

        public static async Task<Breadcrumb> GetFromAppPage(AppPage appPage) 
        {
            var rootPage = await appPage.GetRootPage();

            var breadcrumbItems = new List<BreadcrumbItem>(8);

            breadcrumbItems.Add(new BreadcrumbItem(appPage.PageUrl, appPage.Properties.Title));

            if (appPage.HasParentPage()) 
            {
                var parentPage = await appPage.GetParentPage();

                while (parentPage != rootPage && parentPage.HasParentPage()) 
                {
                    breadcrumbItems.Add(new BreadcrumbItem(parentPage.PageUrl, parentPage.Properties.Title));

                    parentPage = await parentPage.GetParentPage();
                }
            }

            if (rootPage != appPage) 
            {
                breadcrumbItems.Add(new BreadcrumbItem(rootPage.PageUrl, rootPage.Properties.Title));
            }

            breadcrumbItems.Reverse();

            return new Breadcrumb(breadcrumbItems);
        }
    }
}
