using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCmsLibrary.Core.Application.UrlRewrite
{
    public class UrlRewriterNone : IUrlRewriter
    {
        public (string Url, string Query) Rewrite(string url, string query)
        {
            return (url, query);
        }
    }
}
