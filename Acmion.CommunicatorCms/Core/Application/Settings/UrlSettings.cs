using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Settings
{
    public static class UrlSettings
    {
        public static string ComponentsRootUrl { get; } = "/Components";

        public static string CmsRootUrl { get; } = "/cms";

        public static string CmsSecondClassUrl { get; } = "/cms/++";

        public static string CmsThirdClassUrl { get; } = "/cms/+++";
        public static string CmsThirdClassApiUrl { get; } = "/cms/+++/api";
        public static string CmsThirdClassThemesUrl { get; } = "/cms/+++/themes";
        public static string CmsThirdClassExtensionsUrl { get; } = "/cms/+++/extensions";


        public static string ContentRootUrl { get; } = "/";

        public static string ContentSecondClassUrl { get; } = "/++";
        public static string ContentSecondClassTranslationsRootUrl { get; } = "/++/translations";

        public static string ContentSecondClassHeadFileUrl { get; } = "/++/head.cshtml";
        public static string ContentSecondClassHeadAppendFileUrl { get; } = "/++/head-append.cshtml";
        public static string ContentSecondClassHeadPrependFileUrl { get; } = "/++/head-prepend.cshtml";

        public static string ContentSecondClassHeaderFileUrl { get; } = "/++/header.cshtml";
        public static string ContentSecondClassHeaderAppendFileUrl { get; } = "/++/header-append.cshtml";
        public static string ContentSecondClassHeaderPrependFileUrl { get; } = "/++/header-prepend.cshtml";

        public static string ContentSecondClassFooterFileUrl { get; } = "/++/footer.cshtml";
        public static string ContentSecondClassFooterAppendFileUrl { get; } = "/++/footer-append.cshtml";
        public static string ContentSecondClassFooterPrependFileUrl { get; } = "/++/footer-prepend.cshtml";

        public static string ContentSecondClassScriptsFileUrl { get; } = "/++/scripts.cshtml";
        public static string ContentSecondClassScriptsAppendFileUrl { get; } = "/++/scripts-append.cshtml";
        public static string ContentSecondClassScriptsPrependFileUrl { get; } = "/++/scripts-prepend.cshtml";

        public static string ContentSecondClassMenuTopUrl { get; } = "/++/menu-top.cshtml";
        public static string ContentSecondClassMenuLeftUrl { get; } = "/++/menu-left.cshtml";
        public static string ContentSecondClassMenuRightUrl { get; } = "/++/menu-right.cshtml";

        public static string ContentThirdClassUrl { get; } = "/+++";
        public static string ContentThirdClassApiUrl { get; } = "/+++/api";
        public static string ContentThirdClassThemesUrl { get; } = "/+++/themes";
        public static string ContentThirdClassExtensionsUrl { get; } = "/+++/extensions";

    }
}
