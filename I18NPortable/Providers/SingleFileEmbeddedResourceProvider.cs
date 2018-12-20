using System.Collections.Generic;
using System.Reflection;

namespace I18NPortable.Providers
{
    internal class SingleFileEmbeddedResourceProvider : EmbeddedResourceProvider
    {
        private string _languageManifestFile;

        public SingleFileEmbeddedResourceProvider(
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
