using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace I18NPortable
{
	public class DefaultLocaleProvider : ILocaleProvider
	{
		public Dictionary<string, string> GetAvailableLocales(Assembly hostAssembly)
		{
			Dictionary<string, string> availableLocales = new Dictionary<string, string>();

			var localeResourceNames = hostAssembly
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

				availableLocales.Add(localeName, resource);
			}

			return availableLocales;
		}

		public Dictionary<string, string> PrepareTranslationForLocale(Assembly hostAssembly, string resourcePath)
		{
			var stream = hostAssembly.GetManifestResourceStream(resourcePath);

			return ParseTranslations(stream);
		}

		private Dictionary<string, string> ParseTranslations(Stream stream)
		{
			Dictionary<string, string> translations = new Dictionary<string, string>();

			using (var streamReader = new StreamReader(stream))
			{
				string key = null;
				string value = null;

				while (!streamReader.EndOfStream)
				{
					var line = streamReader.ReadLine();
					var isEmpty = string.IsNullOrWhiteSpace(line);
					var isComment = !isEmpty && line.Trim().StartsWith("#");
					var isKeyValuePair = !isEmpty && !isComment && line.Contains("=");

					if ((isEmpty || isComment || isKeyValuePair) && key != null && value != null)
					{
						translations.Add(key, value);

						key = null;
						value = null;
					}

					if (isEmpty || isComment)
						continue;

					if (isKeyValuePair)
					{
						var kvp = line.Split(new[] { '=' }, 2);

						key = kvp[0].Trim();
						value = kvp[1].Trim().UnescapeLineBreaks();
					}
					else if (key != null && value != null)
					{
						value = value + Environment.NewLine + line.Trim().UnescapeLineBreaks();
					}
				}

				if (key != null && value != null)
					translations.Add(key, value);
			}

			return translations;
		}

	}
}
