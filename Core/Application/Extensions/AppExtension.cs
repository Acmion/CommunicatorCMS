using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCmsLibrary.Core.Application.FileSystem;
using Acmion.CommunicatorCmsLibrary.Core.Settings;
using Microsoft.AspNetCore.Html;

namespace Acmion.CommunicatorCmsLibrary.Core.Application.Extensions
{
    public abstract class AppExtension
    {
        public string Id { get; }
        public string Name { get; }

        public string CmsSettingsRootUrl => AppUrl.Join(UrlSettings.CmsGeneralSettingsExtensionsUrl, Id);
        public string ContentSettingsRootUrl => AppUrl.Join(UrlSettings.ContentGeneralSettingsExtensionUrl, Id);

        public bool CmsSettingsRootUrlExists => AppUrl.Exists(CmsSettingsRootUrl);
        public bool ContentSettingsRootUrlExists => AppUrl.Exists(ContentSettingsRootUrl);

        public AppExtension(string id, string name) 
        {
            Id = id;
            Name = name;
        }

        public virtual void UnRegister() 
        {
            
        }

        public virtual void OnRequestStart(RequestState requestState) { }
        public virtual void OnRequestEnd(RequestState requestState) { }

        public virtual void OnActionStart(RequestState requestState) { }
        public virtual void OnActionEnd(RequestState requestState) { }

        public virtual void OnCmsStart(RequestState requestState) { }
        public virtual void OnCmsEnd(RequestState requestState) { }

        public virtual void OnContentStart(RequestState requestState) { }
        public virtual void OnContentEnd(RequestState requestState) { }

        public virtual IHtmlContent HeadContent(RequestState requestState) 
        {
            return HtmlString.Empty;
        }
    }
}
