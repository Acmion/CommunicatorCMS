using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acmion.CommunicatorCms.Core.Application.FileSystem;

namespace Acmion.CommunicatorCms.Core.Settings
{
    public static class GeneralSettings
    {
        public static string RazorPagesRootAppPath { get; } = "/Pages";
        public static string ComponentsRootAppPath { get; } = "/Components";
        public static string WwwRootAppPath { get; } = "/wwwroot";

    }
}
