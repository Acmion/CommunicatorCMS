using Acmion.CommunicatorCms.Core.Application.FileSystem;
using Acmion.CommunicatorCms.Core.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acmion.CommunicatorCms.Core.Application.Translations
{
    public class TranslationHandler
    {
        public ConcurrentDictionary<(string Key, string LanguageId), string> Translations { get; } = new ConcurrentDictionary<(string Key, string LanguageId), string>();

        public async Task LoadTranslations() 
        {
            var files = AppUrl.GetFiles(UrlSettings.ContentSecondClassTranslationsRootUrl, "*.*", System.IO.SearchOption.AllDirectories);

            foreach (var fileAppUrl in files) 
            {
                await LoadTranslationFileFromAppUrl(fileAppUrl);
            }
        }

        public async Task LoadTranslationFileFromAppUrl(string translationFileAppUrl) 
        {
            try
            {
                var fileContent = await AppUrl.ReadAllTextAsync(translationFileAppUrl);
                var translations = App.YamlDeserializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(fileContent);

                foreach (var keyAndLanguageTranslationPair in translations) 
                {
                    var key = keyAndLanguageTranslationPair.Key;

                    foreach (var languageAndTranslationPair in keyAndLanguageTranslationPair.Value) 
                    {
                        var langId = languageAndTranslationPair.Key;
                        var translation = languageAndTranslationPair.Value;

                        Translations[(key, langId)] = translation;
                    }
                }

            }
            catch 
            {

            }
        }

        public string Translate(string key, string languageId) 
        {
            if (Translations.TryGetValue((key, languageId), out var value)) 
            {
                return value;
            }

            return $"ERROR: No translation found for key: \"{key}\"";
        }
    }
}
