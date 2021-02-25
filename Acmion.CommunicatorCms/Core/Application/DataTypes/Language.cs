using System;
using System.Collections.Generic;
using System.Text;

namespace Acmion.CommunicatorCms.Core.Application.DataTypes
{
    public class Language
    {
        public static Language Default { get; } = new Language() { Id = "en", Name = "English" };

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
