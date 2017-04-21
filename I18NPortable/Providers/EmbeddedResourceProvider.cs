using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace I18NPortable.Providers
{
    public class EmbeddedResourceProvider : ILocaleProvider
    {
        private readonly Dictionary<string, string> _locales = new Dictionary<string, string>(); // ie: [es] = "Project.Locales.es.txt"
        private readonly Assembly _hostAssembly;
        private readonly string _resourceFolder;
        private readonly IEnumerable<string> _knownFileExtensions;
        private Action<string> _logger;

        public EmbeddedResourceProvider(Assembly hostAssembly, string resourceFolder = "Locales", 
            IEnumerable<string> knownFileExtensions = null)
        {
            _hostAssembly = hostAssembly;
            _resourceFolder = resourceFolder;
            _knownFileExtensions = knownFileExtensions ?? new []{ ".txt", ".json" };
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
            DiscoverLocales(_hostAssembly);
            return this;
        }

        public IEnumerable<string> GetAvailableLocales() 
            => _locales.Select(x => x.Key);

        private void DiscoverLocales(Assembly hostAssembly)
        {
            _logger?.Invoke("Getting available locales...");

            var localeResources = hostAssembly
                .GetManifestResourceNames()
                .Where(x => x.Contains($".{_resourceFolder}."));

            var supportedResources = 
                (from name in localeResources
                 from extension in _knownFileExtensions
                 where name.EndsWith(extension)
                 select name)
                 .ToList();

            if (supportedResources.Count == 0)
            {
                throw new Exception("No locales have been found. Make sure you´ve got a folder " +
                                    $"called '{_resourceFolder}' containing embedded resource files" +
                                    $" ({string.Join(", ", _knownFileExtensions)}) " +
                                    "in the host assembly");
            }

            foreach (var resource in supportedResources)
            {
                var parts = resource.Split('.');
                var localeName = parts[parts.Length - 2];

                if (_locales.ContainsKey(localeName))
                {
                    throw new Exception($"The locales folder '{_resourceFolder}' contains a duplicated locale '{localeName}'");
                }

                _locales.Add(localeName, resource);
            }

            _logger?.Invoke($"Found {supportedResources.Count} locales: {string.Join(", ", _locales.Keys.ToArray())}");
        }
    }
}
