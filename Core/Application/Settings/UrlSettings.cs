using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCmsLibrary.Core.Settings
{
    public static class UrlSettings
    {
        public static string CmsRootUrl { get; } = "/cms";

        public static string CmsCommunicatorCmsUrl { get; } = "/cms/communicator-cms";
        public static string CmsCommunicatorCmsApiUrl { get; } = "/cms/communicator-cms/api";
        public static string CmsCommunicatorCmsThemesUrl { get; } = "/cms/communicator-cms/_themes";
        public static string CmsCommunicatorCmsExtensionsUrl { get; } = "/cms/communicator-cms/_extensions";

        public static string CmsGeneralUrl { get; } = "/cms/_general";
        public static string CmsGeneralSettingsUrl { get; } = "/cms/_general/settings";
        public static string CmsGeneralSettingsExtensionsUrl { get; } = "/cms/_general/settings/extensions";

        public static string WebRootUrl { get; } = "/";

        public static string WebCommunicatorCmsUrl { get; } = "/communicator-cms";
        public static string WebCommunicatorCmsApiUrl { get; } = "/communicator-cms/api";
        public static string WebCommunicatorCmsThemesUrl { get; } = "/communicator-cms/_themes";
        public static string WebCommunicatorCmsExtensionsUrl { get; } = "/communicator-cms/_extensions";

        public static string WebGeneralUrl { get; } = "/_general";
        public static string WebGeneralSettingsUrl { get; } = "/_general/settings";
        public static string WebGeneralSettingsExtensionUrl { get; } = "/_general/settings/extensions";
    }
}
