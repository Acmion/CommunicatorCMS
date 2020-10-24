using System;
using System.Collections.Generic;
using System.Text;

namespace Acmion.CommunicatorCms.Core.Application.Utilities
{
    public class BreadcrumbItem
    {
        public string Url { get; set; }
        public string Title { get; set; }

        public BreadcrumbItem(string url, string title) 
        {
            Url = url;
            Title = title;
        }
    }
}
