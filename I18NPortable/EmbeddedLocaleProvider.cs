using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace I18NPortable
{
	public class EmbeddedLocaleProvider : ILocaleProvider
	{
        private Assembly _hostAssembly;

        public EmbeddedLocaleProvider(Assembly hostAssembly)
        {
            _hostAssembly = hostAssembly;
        }

		public Dictionary<string, Strategies.LocaleStrategieCollection> GetAvailableLocales()
		{
			Dictionary<string, Strategies.LocaleStrategieCollection> availableLocales = new Dictionary<string, Strategies.LocaleStrategieCollection>();

			var localeResourceNames = _hostAssembly
                .GetManifestResourceNames()
				.Where(x => x.Contains("Locales.") && x.EndsWith(".txt"))
				.ToArray();

			if (localeResourceNames.Length == 0)
			{
				throw new Exception("No locales have been found. Make sure you´ve got a folder " +
									"called 'Locales' containing .txt files in the host PCL assembly");
			}

			foreach (var resource in localeResourceNames)
			{
				var parts = resource.Split('.');
				var localeName = parts[parts.Length - 2];

				availableLocales.Add(localeName, new Strategies.LocaleStrategieCollection( new Strategies.EmbeddedLocaleReceiveStrategy(_hostAssembly, resource)));
			}

			return availableLocales;
		}
	}
}
