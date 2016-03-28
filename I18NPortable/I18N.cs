using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace I18NPortable
{
	// TODO implement INotifyPropertyChanged (for CurrentLanguage, Languages)?
	// TODO implement bindable indexer for real time updates?
	public class I18N 
	{
		#region Singleton

		public static I18N Current => I18NInstance.Value;

		// Lazy initialization ensures I18NPortableInstance creation is threadsafe
		private static readonly Lazy<I18N> I18NInstance = new Lazy<I18N>(() => new I18N());

		#endregion

		public string this[string key] => Translate(key);

		public PortableLanguage CurrentLanguage
		{
			get
			{
				return Languages.FirstOrDefault(x => x.Locale.Equals(_currentLocale));
			}
			set
			{
				LoadLocale(value.Locale); 
				// TODO notifypropertychanged
			}
		}

		private string _currentLocale;
		public string CurrentLocale
		{
			get { return _currentLocale; }
			set
			{
				LoadLocale(value);
			}
		}

		private List<PortableLanguage> _languages;
		public IEnumerable<PortableLanguage> Languages
		{
			get
			{
				if(_languages != null) return _languages;

				var languages = _locales.Select(x => new PortableLanguage
					{ Locale = x.Key, DisplayName = TranslateOrNull(x.Key) ?? new CultureInfo(x.Key).NativeName.CapitalizeFirstLetter() })
					.ToList();

				if (languages.Count > 0)
					_languages = languages;

				return _languages;
			}
		}

		private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();
		private readonly Dictionary<string, string> _locales = new Dictionary<string, string>();
		private Assembly _hostAssembly;
		private bool _logEnabled = true;
		private bool _throwWhenKeyNotFound;
		private string _notFoundSymbol = "?";
		private string _fallbackLocale;

		private I18N() {}

		#region Fluent API

		/// <summary>
		/// Set the symbol to wrap a key when not found. ie: if you set "##", a not found key will
		/// be translated as "##key##". 
		/// The default symbol is "?"
		/// </summary>
		public I18N SetNotFoundSymbol(string symbol)
		{
			if (!string.IsNullOrEmpty(symbol))
				_notFoundSymbol = symbol;
			return this;
		}

		/// <summary>
		/// Enable/disable log traces
		/// </summary>
		public I18N SetLogEnabled(bool enabled)
		{
			_logEnabled = enabled;
			return this;
		}

		/// <summary>
		/// Throw an exception whenever a key is not found in the locale file (fail early, fail fast)
		/// </summary>
		public I18N SetThrowWhenKeyNotFound(bool enabled)
		{
			_throwWhenKeyNotFound = enabled;
			return this;
		}

		/// <summary>
		/// Set the locale that will be loaded in case the system language is not supported
		/// </summary>
		public I18N SetFallbackLocale(string locale)
		{
			_fallbackLocale = locale;
			return this;
		}

		/// <summary>
		/// Call this at your app initialiation 
		/// ie: I18N.Current.Init(GetType().GetTypeInfo().Assembly);
		/// </summary>
		/// <param name="hostAssembly">The PCL assembly that hosts the locale text files</param>
		public I18N Init(Assembly hostAssembly)
		{
			DiscoverLocales(hostAssembly);

			_hostAssembly = hostAssembly;

			var localeToLoad = GetDefaultLocaleFromCurrentCulture();

			if (string.IsNullOrEmpty(localeToLoad))
			{
				if (!string.IsNullOrEmpty(_fallbackLocale) && _locales.ContainsKey(_fallbackLocale))
				{
					localeToLoad = _fallbackLocale;
					Log($"Loading fallback locale: {_fallbackLocale}");
				}
				else
				{
					localeToLoad = _locales.Keys.ToArray()[0];
					Log($"Loading first locale on the list: {localeToLoad}");
				}
			}
			else
			{
				Log($"Default locale from current culture: {localeToLoad}");
			}	

			LoadLocale(localeToLoad);

			return this;
		}

		#endregion

		#region Load stuff

		private void DiscoverLocales(Assembly hostAssembly)
		{
			Log("Getting available locales...");

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
				
				_locales.Add(localeName, resource);
			}

			Log($"Found {localeResourceNames.Length} locales: {string.Join(", ", _locales.Keys.ToArray())}");
		}

		public void LoadLanguage(PortableLanguage portableLanguage) => LoadLocale(portableLanguage.Locale);

		public void LoadLocale(string locale)
		{
			if (!_locales.ContainsKey(locale))
				throw new KeyNotFoundException($"Locale '{locale}' is not available");

			var resourceName = _locales[locale];
			var stream = _hostAssembly.GetManifestResourceStream(resourceName);

			LoadTranslations(stream);

			_currentLocale = locale;

			// OnPropertyChanged("Item[]"); // TODO real time updates
		}

		private void LoadTranslations(Stream stream)
		{
			using (var streamReader = new StreamReader(stream))
			{
				while (!streamReader.EndOfStream)
				{
					var line = streamReader.ReadLine();
					if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
						continue;

					var columns = line.Split(new[] { '=' }, 2);
					_translations.Add(columns[0].Trim(), columns[1].Trim());
				}
			}

			LogTranslations();
		}

		#endregion

		#region Translations

		public string Translate(string key, params object[] args)
		{
			if (_translations.ContainsKey(key))
				return args.Length == 0 ? _translations[key] : string.Format(_translations[key], args);

			if(_throwWhenKeyNotFound)
				throw new KeyNotFoundException($"[{nameof(I18N)}] key '{key}' not found in the current language '{_currentLocale}'");

			return $"{_notFoundSymbol}{key}{_notFoundSymbol}";
		}

		public string TranslateOrNull(string key, params object[] args) => 
			_translations.ContainsKey(key) 
				? (args.Length == 0 ? _translations[key] : string.Format(_translations[key], args)) 
				: null;

		public Dictionary<T, string> TranslateEnum<T>()
		{
			var type = typeof(T);
			var dic = new Dictionary<T, string>();

			foreach (var value in Enum.GetValues(type))
			{
				var name = Enum.GetName(type, value);
				dic.Add((T)value, Translate($"{type.Name}.{name}"));
			}

			return dic;
		}

		#endregion

		#region Helpers

		private string GetDefaultLocaleFromCurrentCulture()
		{
			var currentCulture = CultureInfo.CurrentCulture;

			// only available in runtime (not from PCL) // TODO (runtime properties are working in iOS, test other platforms)
			var threeLetterIsoName = currentCulture.GetType().GetRuntimeProperty("ThreeLetterISOLanguageName").GetValue(currentCulture);
			var threeLetterWindowsName = currentCulture.GetType().GetRuntimeProperty("ThreeLetterWindowsLanguageName").GetValue(currentCulture);

			var valuePair = _locales.FirstOrDefault(x => x.Key.Equals(currentCulture.Name) // i.e: "es-ES", "en-US"
				|| x.Key.Equals(currentCulture.TwoLetterISOLanguageName) // ISO 639-1 two-letter code. i.e: "es"
				|| x.Key.Equals(threeLetterIsoName) // ISO 639-2 three-letter code. i.e: "spa"
				|| x.Key.Equals(threeLetterWindowsName)); // "ESP"

			return valuePair.Key;
		}

		#endregion

		#region Logging

		private void LogTranslations()
		{
			if (!_logEnabled) return;

			Log("========== I18N translations ==========");
			foreach (var item in _translations)
				Log($"{item.Key} = {item.Value}");
		}

		private void Log(string trace)
		{
			if (_logEnabled)
				Debug.WriteLine($"[{nameof(I18N)}] {trace}");
		}

		#endregion
	}
}