using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using I18NPortable.Providers;
using I18NPortable.Readers;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace I18NPortable
{
    public class I18N : II18N
    {
        public static II18N Current { get; set; } = new I18N();

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
            get => Languages?.FirstOrDefault(x => x.Locale.Equals(Locale));
            set
            {
                if (Language.Locale == value.Locale)
                {
                    Log($"{value.DisplayName} is the current language. No actions will be taken");
                    return;
                }

                LoadLocale(value.Locale);

                NotifyPropertyChanged(nameof(Locale));
                NotifyPropertyChanged(nameof(Language));
            }
        }

        private string _locale;

        /// <summary>
        /// The current loaded locale name (can be two letter ISO-code or a culture name like "es-ES")
        /// </summary>
        public string Locale
        {
            get => _locale;
            set
            {
                if (_locale == value)
                {
                    Log($"{value} is the current locale. No actions will be taken");
                    return;
                }

                LoadLocale(value);

                NotifyPropertyChanged(nameof(Locale));
                NotifyPropertyChanged(nameof(Language));
            }
        }

        /// <summary>
        /// A list of supported languages
        /// </summary>
        public List<PortableLanguage> Languages => _locales?.Select(x => new PortableLanguage
        {
            Locale = x,
            DisplayName = TranslateOrNull(x) ?? new CultureInfo(x).NativeName.CapitalizeFirstCharacter()
        })
        .ToList();

        private Dictionary<string, string> _translations = new Dictionary<string, string>();
        private readonly IList<ILocaleProvider> _providers = new List<ILocaleProvider>();
        private readonly IList<Tuple<ILocaleReader, string>> _readers = new List<Tuple<ILocaleReader, string>>();
        private IList<string> _locales = new List<string>();
        private Dictionary<string, string> _localeFileExtensionMap;
        private bool _throwWhenKeyNotFound;
        private string _notFoundSymbol = "?";
        private string _fallbackLocale;
        private Action<string> _logger;
        private string _resourcesFolder;

        #region Fluent API

        /// <summary>
        /// Set the symbol to wrap a key when not found. ie: if you set "##", a not found key will
        /// be translated as "##key##". 
        /// The default symbol is "?"
        /// </summary>
        public II18N SetNotFoundSymbol(string symbol)
        {
            if (!string.IsNullOrEmpty(symbol))
                _notFoundSymbol = symbol;
            return this;
        }

        /// <summary>
        /// Enable I18N logs with an action
        /// </summary>
        /// <param name="output">Action to be invoked as the output of the logger</param>
        public II18N SetLogger(Action<string> output)
        {
            _logger = output;
            return this;
        }

        /// <summary>
        /// Throw an exception whenever a key is not found in the locale file (fail early, fail fast)
        /// </summary>
        public II18N SetThrowWhenKeyNotFound(bool enabled)
        {
            _throwWhenKeyNotFound = enabled;
            return this;
        }

        /// <summary>
        /// Set the locale that will be loaded in case the system language is not supported
        /// </summary>
        public II18N SetFallbackLocale(string locale)
        {
            _fallbackLocale = locale;
            return this;
        }

        public II18N SetResourcesFolder(string folderName)
        {
            _resourcesFolder = folderName;
            return this;
        }

        public II18N AddLocaleReader(ILocaleReader reader, string extension)
        {
            if(reader == null)
                throw new I18NException(ErrorMessages.ReaderNull);

            if(string.IsNullOrEmpty(extension))
                throw new I18NException(ErrorMessages.ReaderExtensionNeeded);

            if(!extension.StartsWith("."))
                throw new I18NException(ErrorMessages.ReaderExtensionStartWithDot);

            if(extension.Length < 2)
                throw new I18NException(ErrorMessages.ReaderExtensionOneChar);

            if(extension.Split('.').Length - 1 > 1)
                throw new I18NException(ErrorMessages.ReaderExtensionJustOneDot);

            if(_readers.Any(x => x.Item2.Equals(extension)))
                throw new I18NException(ErrorMessages.ReaderExtensionTwice);

            if(_readers.Any(x => x.Item1 == reader))
                throw new I18NException(ErrorMessages.ReaderTwice);

            _readers.Add(new Tuple<ILocaleReader, string>(reader, extension));

            return this;
        }

        /// <summary>
        /// Call this when your app starts
        /// ie: <code>I18N.Current.Init(GetType().GetTypeInfo().Assembly);</code>
        /// </summary>
        /// <param name="hostAssembly">The assembly that hosts the locale text files</param>
        public II18N Init(Assembly hostAssembly)
        {
            if (_readers.FirstOrDefault(x => x.Item1 is TextKvpReader && x.Item2 == ".txt") == null)
            {
                _readers.Insert(0, new Tuple<ILocaleReader, string>(new TextKvpReader(), ".txt"));
            }

            var knownFileExtensions = _readers.Select(x => x.Item2);

            foreach (var provider in _providers)
            {
                provider.Dispose();
            }

            _providers.Clear(); // temporal until other providers are implemented

            if (_providers.FirstOrDefault(x => x is EmbeddedResourceProvider) == null)
            {
                var resourcesFolder = _resourcesFolder ?? "Locales";
                var defaultProvider = new EmbeddedResourceProvider(hostAssembly, resourcesFolder, knownFileExtensions)
                    .SetLogger(Log)
                    .Init();

                _providers.Add(defaultProvider);
            }

            var localeTuples = _providers.First().GetAvailableLocales().ToList();
            _locales = localeTuples.Select(x => x.Item1).ToList();
            _localeFileExtensionMap = localeTuples.ToDictionary(x => x.Item1, x => x.Item2);

            var localeToLoad = GetDefaultLocale();

            if (string.IsNullOrEmpty(localeToLoad))
            {
                if (!string.IsNullOrEmpty(_fallbackLocale) && _locales.Contains(_fallbackLocale))
                {
                    localeToLoad = _fallbackLocale;
                    Log($"Loading fallback locale: {_fallbackLocale}");
                }
                else
                {
                    localeToLoad = _locales.ElementAt(0);
                    Log($"Loading first locale on the list: {localeToLoad}");
                }
            }
            else
            {
                Log($"Default locale from current culture: {localeToLoad}");
            }

            LoadLocale(localeToLoad);

            NotifyPropertyChanged(nameof(Locale));
            NotifyPropertyChanged(nameof(Language));

            return this;
        }

        #endregion

        #region Load stuff

        private void LoadLocale(string locale)
        {
            if (!_locales.Contains(locale))
                throw new I18NException($"Locale '{locale}' is not available", new KeyNotFoundException());

            // TODO try get from all providers in the correct order
            var stream = _providers.First().GetLocaleStream(locale);

            _translations.Clear();

            var extension = _localeFileExtensionMap[locale];
            var reader = _readers.First(x => x.Item2.Equals(extension)).Item1;

            try
            {
                _translations = reader.Read(stream) ?? new Dictionary<string, string>();
            }
            catch (Exception e)
            {
                var message = $"{ErrorMessages.ReaderException}.\nReader: {reader.GetType().Name}.\nLocale: {locale}{extension}";
                throw new I18NException(message, e);
            }
            
            LogTranslations();

            _locale = locale;

            // Update bindings to indexer (useful for MVVM)
            NotifyPropertyChanged("Item[]");
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

            if (_throwWhenKeyNotFound)
                throw new KeyNotFoundException(
                    $"[{nameof(I18N)}] key '{key}' not found in the current language '{_locale}'");

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

        public II18NSection GetSection(string section)
        {
            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentNullException(nameof(section));
            }
            
            return new I18NSection(this, section);
        }

        /// <summary>
        /// Convert Enum Type values to a Dictionary&lt;TEnum, string&gt; where the key is the Enum value and the string is the translated value.
        /// </summary>
        public Dictionary<TEnum, string> TranslateEnumToDictionary<TEnum>(string section)
        {
            var type = typeof(TEnum);
            var dic = new Dictionary<TEnum, string>();
            var prefix = string.IsNullOrEmpty(section) ? type.Name : $"{section}.{type.Name}";

            foreach (var value in Enum.GetValues(type))
            {
                var name = Enum.GetName(type, value);
                dic.Add((TEnum) value, Translate($"{prefix}.{name}"));
            }

            return dic;
        }

        /// <summary>
        /// Convert Enum Type values to a List of translated strings
        /// </summary>
        public List<string> TranslateEnumToList<TEnum>(string section = null)
        {
            var type = typeof(TEnum);
            var prefix = string.IsNullOrEmpty(section) ? type.Name : $"{section}.{type.Name}";

            return (from object value in Enum.GetValues(type)
                    select Enum.GetName(type, value)
                    into name
                    select Translate($"{prefix}.{name}"))
                .ToList();
        }

        /// <summary>
        /// Converts Enum Type values to a List of <code>Tuple&lt;TEnum, string&gt;</code> where the Item2 (string) is the enum value translation
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public List<Tuple<TEnum, string>> TranslateEnumToTupleList<TEnum>(string section)
        {
            var type = typeof(TEnum);
            var list = new List<Tuple<TEnum, string>>();
            var prefix = string.IsNullOrEmpty(section) ? type.Name : $"{section}.{type.Name}";
            
            foreach (var value in Enum.GetValues(type))
            {
                var name = Enum.GetName(type, value);
                var tuple = new Tuple<TEnum, string>((TEnum) value, Translate($"{prefix}.{name}"));
                list.Add(tuple);
            }

            return list;
        }

        #endregion

        #region Helpers

        public string GetDefaultLocale()
        {
            var currentCulture = CultureInfo.CurrentCulture;

            // only available in runtime (not from PCL) on the simulator
            // var threeLetterIsoName = currentCulture.GetType().GetRuntimeProperty("ThreeLetterISOLanguageName").GetValue(currentCulture);
            // var threeLetterWindowsName = currentCulture.GetType().GetRuntimeProperty("ThreeLetterWindowsLanguageName").GetValue(currentCulture);

            var matchingLocale = _locales.FirstOrDefault(x => x.Equals(currentCulture.Name));

            if (matchingLocale == null)
            {
                matchingLocale = _locales.FirstOrDefault(x => x.Equals(currentCulture.TwoLetterISOLanguageName));
            }

            return matchingLocale;

                // ISO 639-1 two-letter code. i.e: "es"
            // || x.Key.Equals(threeLetterIsoName) // ISO 639-2 three-letter code. i.e: "spa"
            // || x.Key.Equals(threeLetterWindowsName)); // "ESP"
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

        #region Cleanup

        public void Dispose()
        {
            if (PropertyChanged != null)
            {
                foreach (var @delegate in PropertyChanged.GetInvocationList())
                {
                    PropertyChanged -= (PropertyChangedEventHandler)@delegate;
                }

                PropertyChanged = null;
            }

            _translations?.Clear();
            _locales?.Clear();
            _readers?.Clear();
            _localeFileExtensionMap?.Clear();
            _locale = null;

            Current = null;

            Log("Disposed");

            _logger = null;
        }

        #endregion
    }
}