using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global

namespace I18NPortable
{
	public class I18N : INotifyPropertyChanged
	{
		#region Singleton

        /// <summary>
        /// Unique instance (Singleton) of I18N
        /// </summary>
		public static I18N Current => I18NInstance.Value;

		// Lazy initialization ensures I18NPortableInstance creation is threadsafe
		private static readonly Lazy<I18N> I18NInstance = new Lazy<I18N>(() => new I18N());

		#endregion

		// PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string info) => 
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));

        /// <summary>
        /// Use the indexer to translate keys. If you need string formatting, use <code>Translate()</code> instead
        /// </summary>
        public string this[string key] 
            => Translate(key);

        /// <summary>
        /// The current loaded Language, if any
        /// </summary>
		public PortableLanguage Language
		{
			get { return Languages.FirstOrDefault(x => x.Locale.Equals(Locale)); }
			set
			{
				if (Language.Locale == value.Locale)
				{
					Log($"{value.DisplayName} is the current language. No actions will be taken");
					return;
				}

				LoadLocale(value.Locale);

				NotifyPropertyChanged("Locale");
				NotifyPropertyChanged("Language");
			}
		}

		private string _locale;

        /// <summary>
        /// The current loaded locale 2 letter string
        /// </summary>
		public string Locale
		{
			get { return _locale; }
			set
			{
				if (_locale == value)
				{
					Log($"{value} is the current locale. No actions will be taken");
					return;
				}

				LoadLocale(value);

				NotifyPropertyChanged("Locale");
				NotifyPropertyChanged("Language");
			}
		}

		private List<PortableLanguage> _languages;

        /// <summary>
        /// A list of supported languages
        /// </summary>
		public List<PortableLanguage> Languages
		{
			get
			{
				if(_languages != null)
                    return _languages;

				var languages = _locales.Select(x => new PortableLanguage
				{
                    Locale = x.Key,
                    DisplayName = TranslateOrNull(x.Key) 
                        ?? new CultureInfo(x.Key).NativeName.CapitalizeFirstCharacter()
                })
				.ToList();

				if (languages.Count > 0)
					_languages = languages;

				return _languages;
			}
		}

		private Dictionary<string, string> _translations;
		private Dictionary<string, string> _locales;
		private Assembly _hostAssembly;
		private bool _throwWhenKeyNotFound;
		private string _notFoundSymbol = "?";
		private string _fallbackLocale;
		private Action<string> _logger;

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
		/// Enable I18N logs with an action
		/// </summary>
		/// <param name="output">Action to be invoked as the output of the logger</param>
		public I18N SetLogger(Action<string> output)
		{
			_logger = output;
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
		/// Call this when your app starts
		/// ie: I18N.Current.Init(GetType().GetTypeInfo().Assembly);
		/// </summary>
		/// <param name="hostAssembly">The PCL assembly that hosts the locale text files</param>
		public I18N Init(Assembly hostAssembly)
		{
			Unload();

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

			NotifyPropertyChanged("Locale");
			NotifyPropertyChanged("Language");

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

		private void LoadLocale(string locale)
		{
			if (!_locales.ContainsKey(locale))
				throw new KeyNotFoundException($"Locale '{locale}' is not available");

			var resourceName = _locales[locale];
			var stream = _hostAssembly.GetManifestResourceStream(resourceName);

			ParseTranslations(stream);

			_locale = locale;

			// Update bindings to indexer (useful for views in MVVM frameworks)
			NotifyPropertyChanged("Item[]");
		}

		private void ParseTranslations(Stream stream)
		{
			_translations.Clear();

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
                        _translations.Add(key, value);

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
				    else if(key != null && value != null)
				    {
				        value = value + Environment.NewLine + line.Trim().UnescapeLineBreaks();
				    }
				}

                if(key != null && value != null)
                    _translations.Add(key, value);
            }

			LogTranslations();
		}

		#endregion

		#region Translations

        /// <summary>
        /// Get a translation from a key, formatting the string with the given params, if any
        /// </summary>
		public string Translate(string key, params object[] args)
		{
			if (_translations.ContainsKey(key))
				return args.Length == 0 
                    ? _translations[key]
                    : string.Format(_translations[key], args);

			if(_throwWhenKeyNotFound)
				throw new KeyNotFoundException($"[{nameof(I18N)}] key '{key}' not found in the current language '{_locale}'");

			return $"{_notFoundSymbol}{key}{_notFoundSymbol}";
		}

        /// <summary>
        /// Get a translation from a key, formatting the string with the given params, if any. 
        /// It will return null when the translation is not found
        /// </summary>
		public string TranslateOrNull(string key, params object[] args) => 
			_translations.ContainsKey(key) 
				? (args.Length == 0 ? _translations[key] : string.Format(_translations[key], args)) 
				: null;

        /// <summary>
        /// Get the translation for a given Type name or Type full name.
        /// 
        /// i.e: Given a class <code>MainScreen</code>, <code>Translate<MainScreen>()</code> will search a key "MainScreen" in the locale file.
        /// If not found, it will search for a key [Namespace].MainScreen
        /// </summary>
	    public string Translate<T>()
	    {
	        var type = typeof(T);

            var nameTranslation = type.Name.TranslateOrNull();
            if (nameTranslation != null)
                return nameTranslation;

            var fullNameTranslation = type.FullName.TranslateOrNull();
            return fullNameTranslation ?? type.Name.Translate();
        }

        [Obsolete("This method is deprecated and will be removed on future versions. Please, use 'TranslateEnumToDictionary' to get the same result. In most cases, TranslateEnumToTupleList will be of more help")]
	    public Dictionary<TEnum, string> TranslateEnum<TEnum>()
	        => TranslateEnumToDictionary<TEnum>();

        /// <summary>
        /// Convert Enum Type values to a Dictionary<TEnum, string> where the key is the Enum value and the string is the translated value.
        /// </summary>
        public Dictionary<TEnum, string> TranslateEnumToDictionary<TEnum>()
        {
            var type = typeof(TEnum);
            var dic = new Dictionary<TEnum, string>();

            foreach (var value in Enum.GetValues(type))
            {
                var name = Enum.GetName(type, value);
                dic.Add((TEnum)value, Translate($"{type.Name}.{name}"));
            }

            return dic;
        }

        /// <summary>
        /// Convert Enum Type values to a List of translated strings
        /// </summary>
        public List<string> TranslateEnumToList<TEnum>()
        {
            var type = typeof(TEnum);

            return (from object value in Enum.GetValues(type)
                    select Enum.GetName(type, value) into name
                    select Translate($"{type.Name}.{name}"))
                    .ToList();
        }

        /// <summary>
        /// Converts Enum Type values to a List of Tuple<TEnum, string> where the Item2 (string) is the enum value translation
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
	    public List<Tuple<TEnum, string>> TranslateEnumToTupleList<TEnum>()
	    {
            var type = typeof(TEnum);
            var list = new List<Tuple<TEnum, string>>();

            foreach (var value in Enum.GetValues(type))
            {
                var name = Enum.GetName(type, value);
                var tuple = new Tuple<TEnum, string>((TEnum)value, Translate($"{type.Name}.{name}"));
                list.Add(tuple);
            }

	        return list;
	    }

        #endregion

        #region Helpers

        private string GetDefaultLocaleFromCurrentCulture()
		{
			var currentCulture = CultureInfo.CurrentCulture;

			// only available in runtime (not from PCL) on the simulator
			// var threeLetterIsoName = currentCulture.GetType().GetRuntimeProperty("ThreeLetterISOLanguageName").GetValue(currentCulture);
			// var threeLetterWindowsName = currentCulture.GetType().GetRuntimeProperty("ThreeLetterWindowsLanguageName").GetValue(currentCulture);

			var valuePair = _locales.FirstOrDefault(x => x.Key.Equals(currentCulture.Name) // i.e: "es-ES", "en-US"
				|| x.Key.Equals(currentCulture.TwoLetterISOLanguageName)); // ISO 639-1 two-letter code. i.e: "es"
				// || x.Key.Equals(threeLetterIsoName) // ISO 639-2 three-letter code. i.e: "spa"
				// || x.Key.Equals(threeLetterWindowsName)); // "ESP"

			return valuePair.Key;
		}

		#endregion

		#region Logging

		private void LogTranslations()
		{
			Log("========== I18NPortable translations ==========");
			foreach (var item in _translations)
				Log($"{item.Key} = {item.Value}");
            Log("====== I18NPortable end of translations =======");
        }
		 
		private void Log(string trace) 
            => _logger?.Invoke($"[{nameof(I18N)}] {trace}");

        #endregion

        public void Unload()
		{
            _translations = new Dictionary<string, string>();
            _locales = new Dictionary<string, string>();

            Log("Unloaded");
		}
	}
}