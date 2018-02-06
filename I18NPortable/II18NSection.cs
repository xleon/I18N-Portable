namespace I18NPortable
{
    using System;
    using System.Collections.Generic;

    public interface II18NSection
    {
        II18N I18N { get; }
        
        string Name { get; }

        string this[string key] { get; }
        string Translate(string key, params object[] args);
        string TranslateOrNull(string key, params object[] args);

        Dictionary<TEnum, string> TranslateEnumToDictionary<TEnum>();
        List<string> TranslateEnumToList<TEnum>();
        List<Tuple<TEnum, string>> TranslateEnumToTupleList<TEnum>();
    }
}