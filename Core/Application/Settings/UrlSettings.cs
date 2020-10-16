using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCmsLibrary.Core.Settings
{
    public static class UrlSettings
    {
        public static string ComponentsRootUrl { get; } = "/Components";

        public static string CmsRootUrl { get; } = "/cms";

        public static string CmsCommunicatorCmsUrl { get; } = "/cms/communicator-cms";
        public static string CmsCommunicatorCmsApiUrl { get; } = "/cms/communicator-cms/api";
        public static string CmsCommunicatorCmsThemesUrl { get; } = "/cms/communicator-cms/_themes";
        public static string CmsCommunicatorCmsExtensionsUrl { get; } = "/cms/communicator-cms/_extensions";

        public static string CmsGeneralUrl { get; } = "/cms/_general";
        public static string CmsGeneralSettingsUrl { get; } = "/cms/_general/settings";
        public static string CmsGeneralSettingsExtensionsUrl { get; } = "/cms/_general/settings/extensions";

        public static string ContentRootUrl { get; } = "/";

        public static string ContentCommunicatorCmsUrl { get; } = "/communicator-cms";
        public static string ContentCommunicatorCmsApiUrl { get; } = "/communicator-cms/api";
        public static string ContentCommunicatorCmsThemesUrl { get; } = "/communicator-cms/_themes";
        public static string ContentCommunicatorCmsExtensionsUrl { get; } = "/communicator-cms/_extensions";

        public static string ContentGeneralUrl { get; } = "/_general";
        public static string ContentGeneralSettingsUrl { get; } = "/_general/settings";
        public static string ContentGeneralSettingsExtensionUrl { get; } = "/_general/settings/extensions";

        public static string ContentGeneralFooterFileUrl = "/_general/footer/footer.cshtml";
        public static string ContentGeneralHeadFileUrl = "/_general/head/head.cshtml";
        public static string ContentGeneralMetaFileUrl = "/_general/meta/meta.cshtml";
        public static string ContentGeneralScriptsFileUrl = "/_general/scripts/scripts.cshtml";

    }
}
