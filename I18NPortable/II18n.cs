using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace I18NPortable
{
    public interface II18N : INotifyPropertyChanged
    {
        string this[string key] { get; }
        PortableLanguage Language { get; set; }
        string Locale { get; set; }
        List<PortableLanguage> Languages { get; }

        I18N SetNotFoundSymbol(string symbol);
        I18N SetLogger(Action<string> output);
        I18N SetThrowWhenKeyNotFound(bool enabled);
        I18N SetFallbackLocale(string locale);
        I18N Init(Assembly hostAssembly);

        string GetDefaultLocaleFromCurrentCulture();

        string Translate(string key, params object[] args);
        string TranslateOrNull(string key, params object[] args);

        string Translate<T>();
        Dictionary<TEnum, string> TranslateEnum<TEnum>();
        Dictionary<TEnum, string> TranslateEnumToDictionary<TEnum>();
        List<string> TranslateEnumToList<TEnum>();
        List<Tuple<TEnum, string>> TranslateEnumToTupleList<TEnum>();

        void Unload();
    }
}
