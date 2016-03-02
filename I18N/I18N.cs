using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace I18N
{
    public static class I18N
    {
		public static string[] SupportedLanguages { get; private set; }
		public static string LanguageCode { get; private set; }

		private static Dictionary<string, string> _dictionary;
		private static Assembly _assembly;

	    public static void Init(string[] supportedLanguages, string languageToLoad = null, bool logTranslations = false)
	    {
			if(supportedLanguages == null || supportedLanguages.Length < 1)
				throw new ArgumentException("You must provide an array of language codes");

		    SupportedLanguages = supportedLanguages;

			if(!string.IsNullOrEmpty(languageToLoad))
				LoadLanguage(languageToLoad, logTranslations);
		}

	    public static void LoadLanguage(string languageCode, bool logTranslations = false)
	    {
			if (SupportedLanguages == null || SupportedLanguages.Length < 1)
				throw new ArgumentException("You must provide an array of language codes");

			LanguageCode = !SupportedLanguages.Contains(languageCode) ? SupportedLanguages[0] : languageCode;
			_assembly = _assembly ?? (_assembly = typeof(I18N).GetTypeInfo().Assembly);

			var bundleResource = $"Locales.{LanguageCode}.txt";
			var stream = GetResourceStream(bundleResource);
			if (stream == null) return;

			LoadDictionary(stream);

			if (logTranslations)
				LogTranslations();
		}

		private static Stream GetResourceStream(string resourceName) => _assembly.GetManifestResourceStream(resourceName);

	    public static string Translate(this string key, params object[] args)
		{
			var nonTranslated = "?" + key + "?";

			if (_dictionary == null || !_dictionary.ContainsKey(key)) return nonTranslated;
			return args.Length == 0 ? _dictionary[key] : string.Format(_dictionary[key], args);
		}

		public static string TranslateOrNull(this string key, params object[] args)
		{
			if (_dictionary != null && _dictionary.ContainsKey(key))
			{
				return string.Format(_dictionary[key], args);
			}

			return null;
		}

		public static Dictionary<T, string> TranslateEnum<T>()
		{
			var type = typeof(T);
			var dic = new Dictionary<T, string>();

			foreach (var value in Enum.GetValues(type))
			{
				var name = Enum.GetName(type, value);
				dic.Add((T)value, $"Enums.{type.Name}.{name}".Translate());
			}

			return dic;
		}

		private static void LogTranslations()
		{
			foreach (var item in _dictionary)
				Debug.WriteLine($"{item.Key} = {item.Value}");
		}

		private static void LoadDictionary(Stream stream)
		{
			_dictionary = new Dictionary<string, string>();

			using (var streamReader = new StreamReader(stream))
			{
				while (!streamReader.EndOfStream)
				{
					var line = streamReader.ReadLine();
					if (!string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#"))
					{
						var columns = line.Split(new char[] { '=' }, 2);
						_dictionary.Add(columns[0].Trim(), columns[1].Trim());
					}
				}
			}
		}
	}
}
