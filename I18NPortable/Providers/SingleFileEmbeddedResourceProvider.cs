using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace I18NPortable.Providers
{
    internal class SingleFileEmbeddedResourceProvider : ILocaleProvider
    {
        private readonly Dictionary<string, string> _locales = new Dictionary<string, string>(); // ie: [es] = "Project.Locales.es.txt"
        private readonly Assembly _hostAssembly;
        private readonly string _resourceFolder;
        private Action<string> _logger;

        private readonly string _languageManifestFile;
        private readonly string _resourcesFile;

        public SingleFileEmbeddedResourceProvider(Assembly hostAssembly, string resourceFolder,
            string languageManifestFile, string resourcesFile)
        {
            _hostAssembly = hostAssembly;
            _resourceFolder = resourceFolder;
            _languageManifestFile = languageManifestFile;
            _resourcesFile = resourcesFile;
        }

        public ILocaleProvider SetLogger(Action<string> logger)
        {
            _logger = logger;
            return this;
        }

        public Stream GetLocaleStream(string locale)
        {
            var resourceName = _locales[locale];
            return _hostAssembly.GetManifestResourceStream(resourceName);
        }

        public ILocaleProvider Init()
        {
            ReadLocalesFromManifest(_hostAssembly);

            if (_locales?.Count == 0)
            {
                throw new I18NException($"{ErrorMessages.NoLocalesFound}: {_hostAssembly.FullName}");
            }

            return this;
        }

        public IEnumerable<Tuple<string, string>> GetAvailableLocales() => _locales.Select(x =>
        {
            var extension = x.Value.Substring(x.Value.LastIndexOf('.'));
            return new Tuple<string, string>(x.Key, extension);
        });

        private void ReadLocalesFromManifest(Assembly hostAssembly)
        {
            _logger?.Invoke("Getting available locales...");

            var localeResources = hostAssembly
                .GetManifestResourceNames()
                .Where(x => x.Contains($".{_resourceFolder}."));

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
                                        $"called '{_resourceFolder}' containing embedded resource files " +
                                        "in the host assembly");
            }

            if (resourcesFilePath == null)
            {
                throw new I18NException("Language resources file have not been found. Make sure you´ve got a " +
                                        $"'{_resourcesFile}' file in folder " +
                                        $"called '{_resourceFolder}' containing embedded resource files " +
                                        "in the host assembly");
            }

            var manifestFileStream = _hostAssembly.GetManifestResourceStream(manifestFilePath);
            var streamReader = new StreamReader(manifestFileStream);

            string locale;
            while ((locale = streamReader.ReadLine()) != null)
            {
                _locales.Add(locale, resourcesFilePath);
            }

            _logger?.Invoke($"Found {_locales.Count} locales: {string.Join(", ", _locales.Keys.ToArray())}");
        }

        public void Dispose()
        {
            _locales.Clear();
            _logger = null;
        }
    }
}
