using System;
using System.Collections.Generic;
using System.Text;

namespace Acmion.CommunicatorCmsLibrary.Core.Application.Utilities
{
    public struct BreadcrumbItem
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
