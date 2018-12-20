using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace I18NPortable.Providers
{
    internal class MultiFileEmbeddedResourceProvider : EmbeddedResourceProvider
    {
        private readonly IEnumerable<string> _knownFileExtensions;

        public MultiFileEmbeddedResourceProvider(Assembly hostAssembly, string resourceFolder, IEnumerable<string> knownFileExtensions) 
            : base(hostAssembly, resourceFolder)
        {
            _knownFileExtensions = knownFileExtensions;
        }

        public override ILocaleProvider Init()
        {
            DiscoverLocales(HostAssembly);

            if (Locales?.Count == 0)
            {
                throw new I18NException($"{ErrorMessages.NoLocalesFound}: {HostAssembly.FullName}");
            }

            return this;
        }

        private void DiscoverLocales(Assembly hostAssembly)
        {
            Logger?.Invoke("Getting available locales...");

            var localeResources = hostAssembly
                .GetManifestResourceNames()
                .Where(x => x.Contains($".{ResourceFolder}."));

            var supportedResources = 
                (from name in localeResources
                 from extension in _knownFileExtensions
                 where name.EndsWith(extension)
                 select name)
                 .ToList();

            if (supportedResources.Count == 0)
            {
                throw new I18NException("No locales have been found. Make sure you´ve got a folder " +
                                    $"called '{ResourceFolder}' containing embedded resource files " +
                                    $"(with extensions {string.Join(" or ", _knownFileExtensions)}) " +
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
    }
}
