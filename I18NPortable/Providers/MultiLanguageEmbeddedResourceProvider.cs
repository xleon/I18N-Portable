using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace I18NPortable.Providers
{
    internal class MultiLanguageEmbeddedResourceProvider : EmbeddedResourceProvider
    {
        private string _languageManifestFile;

        public MultiLanguageEmbeddedResourceProvider(
            Assembly hostAssembly, string resourceFolder, string languageManifestFile, IEnumerable<string> knownFileExtensions) 
            : base(hostAssembly, resourceFolder, knownFileExtensions)
        {
            _languageManifestFile = languageManifestFile;
        }

        private new void DiscoverLocales(Assembly hostAssembly)
        {
            Logger?.Invoke("Getting available locales...");

            
        }
    }
}
