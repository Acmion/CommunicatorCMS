using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Application.UrlRewrite
{
    public interface IUrlRewriter
    {
        public abstract (string Url, string Query) Rewrite(string url, string query);
    }

    
}
