using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace I18NPortable.Providers
{
    internal class EmbeddedResourceProvider : ILocaleProvider
    {
        protected readonly Dictionary<string, string> Locales = new Dictionary<string, string>(); // ie: [es] = "Project.Locales.es.txt"
        protected readonly Assembly HostAssembly;
        protected readonly string ResourceFolder;
        protected readonly IEnumerable<string> KnownFileExtensions;
        protected Action<string> Logger;

        public EmbeddedResourceProvider(Assembly hostAssembly, string resourceFolder, IEnumerable<string> knownFileExtensions)
        {
            HostAssembly = hostAssembly;
            ResourceFolder = resourceFolder;
            KnownFileExtensions = knownFileExtensions;
        }

        public ILocaleProvider SetLogger(Action<string> logger)
        {
            Logger = logger;
            return this;
        }

        public Stream GetLocaleStream(string locale)
        {
            var resourceName = Locales[locale];
            return HostAssembly.GetManifestResourceStream(resourceName);
        }

        public ILocaleProvider Init()
        {
            DiscoverLocales(HostAssembly);

            if (Locales?.Count == 0)
            {
                throw new I18NException($"{ErrorMessages.NoLocalesFound}: {HostAssembly.FullName}");
            }

            return this;
        }

        public IEnumerable<Tuple<string, string>> GetAvailableLocales() => Locales.Select(x =>
        {
            var extension = x.Value.Substring(x.Value.LastIndexOf('.'));
            return new Tuple<string, string>(x.Key, extension);
        });

        protected void DiscoverLocales(Assembly hostAssembly)
        {
            Logger?.Invoke("Getting available locales...");

            var localeResources = hostAssembly
                .GetManifestResourceNames()
                .Where(x => x.Contains($".{ResourceFolder}."));

            var supportedResources = 
                (from name in localeResources
                 from extension in KnownFileExtensions
                 where name.EndsWith(extension)
                 select name)
                 .ToList();

            if (supportedResources.Count == 0)
            {
                throw new I18NException("No locales have been found. Make sure you´ve got a folder " +
                                    $"called '{ResourceFolder}' containing embedded resource files " +
                                    $"(with extensions {string.Join(" or ", KnownFileExtensions)}) " +
                                    "in the host assembly");
            }

            foreach (var resource in supportedResources)
            {
                var parts = resource.Split('.');
                var localeName = parts[parts.Length - 2];

                if (Locales.ContainsKey(localeName))
                {
                    throw new I18NException($"The locales folder '{ResourceFolder}' contains a duplicated locale '{localeName}'");
                }

                Locales.Add(localeName, resource);
            }

            Logger?.Invoke($"Found {supportedResources.Count} locales: {string.Join(", ", Locales.Keys.ToArray())}");
        }

        public void Dispose()
        {
            Locales.Clear();
            Logger = null;
        }
    }
}
