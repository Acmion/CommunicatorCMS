using Acmion.CommunicatorCms.Core.Application.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Settings
{
    public class AppSettings
    {
        public string Title { get; set; } = "";
        public string LogoIcon { get; set; } = "";
        public string LogoContent { get; set; } = "";
        
        public string Theme { get; set; } = ThemeSettings.DefaultName;

        public string Version { get; set; } = "";

        public int LinuxLocalHostPort { get; set; } = 5000;

        public Language[] Languages { get; set; } = new Language[0];
        public string[] RootPageUrls { get; set; } = new string[] { "/" };

        public string[][] TranslatedUrlLists { get; set; } = { new string[0] };

        private Dictionary<string, string[]> _urlTranslations = null!;

        public void Initialize() 
        {
            InitializeUrlTranslations();
        }

        public string TranslateUrl(string url, Language language)
        {
            var languageIndex = Array.IndexOf(Languages, language);

            return TranslateUrl(url, languageIndex);
        }

        public string TranslateUrl(string url, string languageId)
        {
            var languageIndex = -1;

            for(var i = 0; i < Languages.Length; i++) 
            {
                if (Languages[i].Id == languageId) 
                {
                    languageIndex = i;
                    break;
                }
            }

            return TranslateUrl(url, languageIndex);
        }

        private void InitializeUrlTranslations() 
        {
            _urlTranslations = new Dictionary<string, string[]>();

            foreach (var urlList in TranslatedUrlLists) 
            {
                foreach (var url in urlList) 
                {
                    _urlTranslations[url] = urlList;
                }
            }
        }

        private string TranslateUrl(string url, int languageIndex) 
        {
            if (languageIndex == -1 || !_urlTranslations.ContainsKey(url))
            {
                return url;
            }

            var currentUrlTranslations = _urlTranslations[url];

            if (currentUrlTranslations.Length <= languageIndex)
            {
                return url;
            }

            return currentUrlTranslations[languageIndex];
        }
    }
}
