using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acmion.CommunicatorCms.Core.Application.Pages
{
    public class AppPageBaseUrlAndParameterValues
    {
        public string BaseUrl { get; }
        public string[] ParameterValues { get; }

        public AppPageBaseUrlAndParameterValues(string baseUrl, string[] parameters) 
        {
            BaseUrl = baseUrl;
            ParameterValues = parameters;
        }
    }
}
