using System;
using System.Collections.Generic;
using System.Text;

namespace Acmion.CommunicatorCms.Core.Application.DataTypes
{
    public class Language
    {
        public static Language Unspecified { get; } = new Language() { Id = "N/A", Name = "N/A" };

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
