using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunicatorCms.Core.Settings
{
    public static class UrlSettings
    {
        public static string CmsRootUrl { get; } = "/cms";
        public static string ContentRootUrl { get; } = "/content";
        public static string RenderingRootUrl { get; } = "/_rendering";

        public static string CmsWwwUrl { get; } = "/cms/www";
        public static string CmsStaticUrl { get; } = "/cms/static";
        public static string CmsActionsUrl { get; } = "/cms/actions";
        public static string CmsGeneralUrl { get; } = "/cms/_general";

        public static string CmsThemesUrl { get; } = "/cms/_general/communicator-cms/themes";
        public static string CmsExtensionsUrl { get; } = "/cms/_general/communicator-cms/extensions";
        public static string CmsExtensionSettingsUrl { get; } = "/cms/_general/settings/extensions";

        public static string ContentWwwUrl { get; } = "/content/www";
        public static string ContentStaticUrl { get; } = "/content/static";
        public static string ContentActionsUrl { get; } = "/content/actions";
        public static string ContentGeneralUrl { get; } = "/content/_general";

        public static string ContentThemesUrl { get; } = "/content/_general/communicator-cms/themes";
        public static string ContentExtensionsUrl { get; } = "/content/_general/communicator-cms/extensions";
        public static string ContentExtensionSettingsUrl { get; } = "/content/_general/settings/extensions";

        public static HashSet<string> NonVirtualRootUrls = new HashSet<string>() { RenderingRootUrl, CmsRootUrl };
        public static HashSet<string> NonVirtualContentUrls = new HashSet<string>() { ContentStaticUrl, ContentActionsUrl, ContentGeneralUrl };
    }
}
