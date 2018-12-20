using System.IO;
using System.Linq;
using System.Reflection;

namespace I18NPortable.Providers
{
    internal class SingleFileEmbeddedResourceProvider : EmbeddedResourceProvider
    {
        private readonly string _languageManifestFile;
        private readonly string _resourcesFile;

        public SingleFileEmbeddedResourceProvider(Assembly hostAssembly, string resourceFolder,
            string languageManifestFile, string resourcesFile) 
            : base(hostAssembly, resourceFolder)
        {
            _languageManifestFile = languageManifestFile;
            _resourcesFile = resourcesFile;
        }

        public override ILocaleProvider Init()
        {
            ReadLocalesFromManifest(HostAssembly);

            if (Locales?.Count == 0)
            {
                throw new I18NException($"{ErrorMessages.NoLocalesFound}: {HostAssembly.FullName}");
            }

            return this;
        }

        private void ReadLocalesFromManifest(Assembly hostAssembly)
        {
            Logger?.Invoke("Getting available locales...");

            var localeResources = hostAssembly
                .GetManifestResourceNames()
                .Where(x => x.Contains($".{ResourceFolder}."));

            var resourcesFilesList = localeResources.ToList();

            string manifestFilePath = null;
            string resourcesFilePath = null;

            foreach (var file in resourcesFilesList)
            {
                if (file.EndsWith(_languageManifestFile))
                {
                    manifestFilePath = file;
                    continue;
                }

                if (file.EndsWith(_resourcesFile))
                {
                    resourcesFilePath = file;
                }
            }

            if (manifestFilePath == null)
            {
                throw new I18NException("Language manifest file have not been found. Make sure you´ve got a " +
                                        $"'{_languageManifestFile}' file in folder " +
                                        $"called '{ResourceFolder}' containing embedded resource files " +
                                        "in the host assembly");
            }

            if (resourcesFilePath == null)
            {
                throw new I18NException("Language resources file have not been found. Make sure you´ve got a " +
                                        $"'{_resourcesFile}' file in folder " +
                                        $"called '{ResourceFolder}' containing embedded resource files " +
                                        "in the host assembly");
            }

            var manifestFileStream = HostAssembly.GetManifestResourceStream(manifestFilePath);
            var streamReader = new StreamReader(manifestFileStream);

            string locale;
            while ((locale = streamReader.ReadLine()) != null)
            {
                Locales.Add(locale, resourcesFilePath);
            }

            Logger?.Invoke($"Found {Locales.Count} locales: {string.Join(", ", Locales.Keys.ToArray())}");
        }
    }
}
