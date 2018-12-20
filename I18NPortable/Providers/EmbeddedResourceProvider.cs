using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace I18NPortable.Providers
{
    internal abstract class EmbeddedResourceProvider :ILocaleProvider
    {
        protected readonly Dictionary<string, string> Locales = new Dictionary<string, string>(); // ie: [es] = "Project.Locales.es.txt"
        protected readonly Assembly HostAssembly;
        protected readonly string ResourceFolder;
        protected Action<string> Logger;

        protected EmbeddedResourceProvider(Assembly hostAssembly, string resourceFolder)
        {
            HostAssembly = hostAssembly;
            ResourceFolder = resourceFolder;
        }

        public ILocaleProvider SetLogger(Action<string> logger)
        {
            Logger = logger;
            return this;
        }

        public abstract ILocaleProvider Init();

        public Stream GetLocaleStream(string locale)
        {
            var resourceName = Locales[locale];
            return HostAssembly.GetManifestResourceStream(resourceName);
        }

        public IEnumerable<Tuple<string, string>> GetAvailableLocales() => Locales.Select(x =>
        {
            var extension = x.Value.Substring(x.Value.LastIndexOf('.'));
            return new Tuple<string, string>(x.Key, extension);
        });

        public void Dispose()
        {
            Locales.Clear();
            Logger = null;
        }
    }
}
