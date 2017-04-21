using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace I18NPortable.Providers
{
    public class EmbeddedLocaleProvider : ILocaleProvider
    {
        private readonly Dictionary<string, string> _locales = new Dictionary<string, string>(); // ie: [es] = "Project.Locales.es.txt"
        private readonly Assembly _hostAssembly;
        private Action<string> _logger;

        public EmbeddedLocaleProvider(Assembly hostAssembly)
        {
            _hostAssembly = hostAssembly;
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

            var localeResourceNames = hostAssembly
                .GetManifestResourceNames()
                .Where(x => x.Contains("Locales.") && x.EndsWith(".txt"))
                .ToArray();

            if (localeResourceNames.Length == 0)
            {
                throw new Exception("No locales have been found. Make sure you´ve got a folder " +
                                    "called 'Locales' containing .txt files in the host assembly");
            }

            foreach (var resource in localeResourceNames)
            {
                var parts = resource.Split('.');
                var localeName = parts[parts.Length - 2];

                _locales.Add(localeName, resource);
            }

            _logger?.Invoke($"Found {localeResourceNames.Length} locales: {string.Join(", ", _locales.Keys.ToArray())}");
        }
    }
}
