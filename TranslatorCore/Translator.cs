using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace TranslatorCore
{
    public interface ITranslator : INotifyPropertyChanged, IDisposable
    {
        string Translate(string key, params object[] args);
        string TranslateFrom(string key, string resource, params object[] args);

        ITranslator ReleaseResource(string resource);
        
        /// <summary>
        /// Set or replace translation overrides, ignoring the current locale
        /// </summary>
        ITranslator SetOverrides(Dictionary<string, string> translationOverrides);
        
        /// <summary>
        /// Add translation overrides, ignoring the current locale
        /// </summary>
        ITranslator AddOverrides(Dictionary<string, string> translationOverrides);
        
        /// <summary>
        /// Use the indexer to translate or override keys.
        /// Note: If you need string formatting, please use <code>Translate()</code> instead or any of the method extensions
        /// You can get a translation by using `Translator.Current[key]`
        /// You can set a translation override with `Translator.Current[key] = "value"`
        /// </summary>
        string this[string key] { get; set; }
        
        string CultureCode { get; set; }
        
        ITranslator Setup(TranslatorSetup setup);
        ITranslator Setup(Func<TranslatorSetup, TranslatorSetup> setupModifierFunc);
    }

    public interface ILocaleLoader
    {
        Dictionary<string, string> Load(string locale, string resource = null);
    }
    
    public class TranslatorException : Exception
    {
        public TranslatorException(string message, Exception innerException = null) : base(message, innerException)
        {
            
        }
    } 

    public class AssemblyKvpResourceLoader : ILocaleLoader
    {
        public AssemblyKvpResourceLoader(Assembly hostAssembly, string directory = "Locales")
        {
            
        }
        
        public Dictionary<string, string> Load(string locale, string resource = null)
        {
            throw new NotImplementedException();
        }
    }

    public class TranslatorSetup
    {
        internal bool ThrowExceptionWhenKeyNotFound { get; private set; }
        internal string NotFoundSymbol { get; private set; } = "?";
        internal string FallbackLocale { get; private set; }
        internal Action<string> Logger { get; private set; }
        internal string[] SupportedLocales { get; private set; }
        internal List<Func<ILocaleLoader>> Loaders { get; } = new List<Func<ILocaleLoader>>();
        internal bool ShouldAutoReleaseResources { get; private set; } = true;

        /// <summary>
        /// Add an `ILocaleLoader` implementation.
        /// Multiple loaders can be added. They will be used in the same order you added them.
        /// If the first of the list fails to load, the next will be used and so on.
        /// </summary>
        public TranslatorSetup AddLoader(ILocaleLoader loader)
        {
            Loaders.Add(() => loader);
            return this;
        }

        public TranslatorSetup AddLoader(Func<ILocaleLoader> loaderFunc)
        {
            Loaders.Add(loaderFunc);
            return this;
        }
        
        /// <summary>
        /// Set the symbol to wrap a key when not found. ie: if you set "##", a not found key will
        /// be translated as "##key##". 
        /// The default symbol is "?"
        /// </summary>
        public TranslatorSetup SetNotFoundSymbol(string symbol)
        {
            NotFoundSymbol = symbol ?? string.Empty;
            return this;
        }
        
        /// <summary>
        /// Enable logs with an action
        /// </summary>
        /// <param name="output">Action to be invoked as the output of the logger</param>
        public TranslatorSetup SetLogger(Action<string> output)
        {
            Logger = output;
            return this;
        }
        
        /// <summary>
        /// Throw an exception whenever a key is not found in the locale file (fail early, fail fast)
        /// </summary>
        public TranslatorSetup ThrowWhenKeyNotFound(bool enabled)
        {
            ThrowExceptionWhenKeyNotFound = enabled;
            return this;
        }
        
        /// <summary>
        /// Set the locale that will be loaded in case the system language is not supported
        /// </summary>
        public TranslatorSetup FallbackWhenCurrentCultureNotSupported(string locale)
        {
            FallbackLocale = locale;
            return this;
        }

        /// <summary>
        /// Array of supported locale codes
        /// </summary>
        public TranslatorSetup SupportLocales(params string[] locales)
        {
            SupportedLocales = locales;
            return this;
        }

        /// <summary>
        /// Release memory allocation of a loaded resource when a new one (non default resource) is loaded
        /// </summary>
        public TranslatorSetup AutoReleaseResources(bool release)
        {
            ShouldAutoReleaseResources = release;
            return this;
        }

        public TranslatorSetup DontReleaseResources(params string[] alwaysKeep)
        {
            throw new NotImplementedException();
        }
    }
    
    public class Translator : ITranslator
    {
        #region Singleton

        private static ITranslator _current;

        public static ITranslator Current
        {
            get => _current ?? (_current = new Translator());
            set
            {
                _current?.Dispose();
                _current = value;
            }
        }

        #endregion
        
        #region INotifyPropertyChanged
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        
        #endregion
        
        #region Indexer and overrides

        public string this[string key]
        {
            get => Translate(key);
            set
            {
                _overrides = _overrides ?? new Dictionary<string, string>();
                _overrides[key] = value;
                
                NotifyPropertyChanged("Item[]");
            }
        }

        private Dictionary<string, string> _overrides;
        
        public ITranslator SetOverrides(Dictionary<string, string> translationOverrides)
        {
            _overrides = translationOverrides;
            NotifyPropertyChanged("Item[]");
            return this;
        }

        public ITranslator AddOverrides(Dictionary<string, string> translationOverrides)
        {
            _overrides = _overrides ?? new Dictionary<string, string>();
            
            foreach (var kvp in translationOverrides)
                _overrides[kvp.Key] = kvp.Value;
            
            NotifyPropertyChanged("Item[]");
            
            return this;
        }
        
        #endregion

        #region Translations

        private readonly Dictionary<string, Dictionary<string, string>> _resources 
            = new Dictionary<string, Dictionary<string, string>>(); // { resource, kvp-dictionary }
        
        public string Translate(string key, params object[] args)
            => TranslateFrom(key, null, args);

        public string TranslateFrom(string key, string resource, params object[] args)
        {
            if (_overrides != null && _overrides.ContainsKey(key))
                return _overrides[key];
            
            if (_setup?.SupportedLocales?.Length < 1)
                throw new TranslatorException(
                    $"Setup missing. Please call {nameof(Setup)}() method before translating any string");

            resource = resource ?? "Default";
            if(!_resources.ContainsKey(resource))
                LoadResource(resource);

//            if (!_translations.ContainsKey(key))
//            {
//                if (_setup.ThrowWhenKeyNotFound)
//                    throw new KeyNotFoundException(
//                        $"[{nameof(Translator)}] key '{key}' not found in the current language '{Locale}'");
//                
//                return $"{_setup.NotFoundSymbol}{key}{_setup.NotFoundSymbol}";
//            }
//            
//            if (_translations.ContainsKey(key))
//                return args.Length == 0
//                    ? _translations[key]
//                    : string.Format(_translations[key], args);

            throw new NotImplementedException();
        }

        #endregion

        #region Resources

        private ITranslator LoadResource(string resource)
        {
            if(_setup?.Loaders == null || _setup.Loaders.Count == 0)
                throw new TranslatorException(
                    $"Could not load resource. Please add a loader on {nameof(TranslatorSetup)}");

            foreach (var loaderFunc in _setup.Loaders)
            {
                string loaderTypeName = null;
                
                try
                {
                    var loader = loaderFunc();
                    
                    if(loader == null)
                        throw new TranslatorException("One of the loader functions is returning null");
                    
                    loaderTypeName = loader.GetType().Name;
                    
                    var translations = loader.Load(CultureCode, resource);
                    _resources[resource] = translations 
                        ?? throw new TranslatorException($"Loader {loader.GetType().Name} returned null when " +
                                                         $"loading resource '{resource}' with locale '{CultureCode}'");
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is TranslatorException)
                    {
                        Log(ex.Message);
                        throw;
                    }
                    
                    var message = $"Loader {loaderTypeName} failed to load " +
                                  $"resource '{resource}' with locale '{CultureCode}'";
                    
                    Log(message);

                    throw new TranslatorException(message, ex);
                }
            }

            return this;
        }
        
        public ITranslator ReleaseResource(string resource)
        {
            if (_resources.ContainsKey(resource))
                _resources.Remove(resource);

            return this;
        }

        #endregion

        #region Clean up

        public void Dispose()
        {
            if (PropertyChanged != null)
            {
                foreach (var @delegate in PropertyChanged.GetInvocationList())
                    PropertyChanged -= (PropertyChangedEventHandler)@delegate;

                PropertyChanged = null;
            }

            _overrides?.Clear();
            _overrides = null;
            
            _resources?.Clear();
            _cultureCode = null;

            Log("Disposed");
            _setup = null;
            _current = null;
        }

        #endregion

        #region Setup

        private TranslatorSetup _setup;
        
        public ITranslator Setup(TranslatorSetup setup)
        {
            _setup = setup;
            Log($"{nameof(TranslatorSetup)} assigned");
            return this;
        }

        public ITranslator Setup(Func<TranslatorSetup, TranslatorSetup> setupModifierFunc) 
            => Setup(setupModifierFunc(_setup ?? new TranslatorSetup()));

        #endregion

        #region Locale

        private string _cultureCode;
        public string CultureCode
        {
            get
            {
                if (_cultureCode != null)
                    return _cultureCode;
                
                if(_setup?.SupportedLocales == null)
                    throw new TranslatorException(
                        $"{nameof(TranslatorSetup)} with {nameof(TranslatorSetup.SupportedLocales)} " +
                        $"must be set before accesing {nameof(CultureCode)}");
                
                // TODO create a CultureInfoProvider that comes from platform to work around iOS / Android specific issues
                var currentCulture = CultureInfo.CurrentCulture;
                var code = (_setup.SupportedLocales.FirstOrDefault(x => x.Equals(currentCulture.Name)) 
                            ?? _setup.SupportedLocales.FirstOrDefault(x => x.Equals(currentCulture.TwoLetterISOLanguageName))) 
                            ?? (_setup.SupportedLocales.Contains(_setup.FallbackLocale)
                               ? _setup.FallbackLocale
                               : _setup.SupportedLocales.First());

                _cultureCode = code;
                
                NotifyPropertyChanged(nameof(CultureCode));
                return _cultureCode;
            }
            set
            {
                if (_cultureCode == value)
                {
                    Log($"{value} is the current {nameof(CultureCode)}. No actions will be taken");
                    return;
                }
                
                throw new NotImplementedException();
            }
        }

        #endregion
        
        #region Logging

        private void LogTranslations()
        {
            if (_setup.Logger == null)
                return;
            
            Log($"========== {nameof(Translator)} translations for locale {CultureCode} ==========");
            foreach (var item in _resources)
            {
                Log($"[{item.Key}]");
                
                foreach(var kvp in item.Value)
                    Log($"{kvp.Key} = {kvp.Value}");
            }
            Log($"========== End of translations  for locale {CultureCode} ==========");
        }

        private void Log(string trace)
            => _setup?.Logger?.Invoke($"[{nameof(Translator)}] {trace}");

        #endregion
    }

    public static class TranslatorExtensions
    {
//        Dictionary<TEnum, string> TranslateEnumToDictionary<TEnum>();
//        List<string> TranslateEnumToList<TEnum>();
//        List<Tuple<TEnum, string>> TranslateEnumToTupleList<TEnum>();
    }
}