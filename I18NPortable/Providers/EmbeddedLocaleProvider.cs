using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace I18NPortable.Providers
{
    public class EmbeddedLocaleProvider : ILocaleProvider
    {
        public Action<string> Logger { get; set; }
        public IEnumerable<string> AvailableLocales => _locales.Select(x => x.Key);

        private readonly Dictionary<string, string> _locales 
            = new Dictionary<string, string>(); // ie: [es] = "Project.Locales.es.txt"

        private readonly Assembly _hostAssembly;

        public EmbeddedLocaleProvider(Assembly hostAssembly, Action<string> logger)
        {
            Logger = logger;

            _hostAssembly = hostAssembly;

            DiscoverLocales(_hostAssembly);
        }

        public Stream GetLocaleStream(string locale)
        {
            var resourceName = _locales[locale];
            return _hostAssembly.GetManifestResourceStream(resourceName);
        }

        private void DiscoverLocales(Assembly hostAssembly)
        {
            Logger?.Invoke("Getting available locales...");

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

            Logger?.Invoke($"Found {localeResourceNames.Length} locales: {string.Join(", ", _locales.Keys.ToArray())}");
        }
    }

    public class RemoteLocaleProvider : ILocaleProvider
    {
        public Action<string> Logger { get; set; }
        public IEnumerable<string> AvailableLocales { get; }

        public Stream GetLocaleStream(string locale)
        {
            
            throw new System.NotImplementedException();
        }
    }
}
