namespace I18NPortable
{
    using System;
    using System.Collections.Generic;

    internal class I18NSection : II18NSection
    {
        public I18NSection(II18N i18N, string section)
        {
            this.I18N = i18N;
            this.Name = section;
        }

        public II18N I18N { get; }

        public string Name { get; }

        public string this[string key] => this.Translate(key);

        public string Translate(string key, params object[] args)
        {
            return this.I18N.Translate(this.BindSection(key), args);
        }

        public string TranslateOrNull(string key, params object[] args)
        {
            return this.I18N.TranslateOrNull(this.BindSection(key), args);
        }

        public Dictionary<TEnum, string> TranslateEnumToDictionary<TEnum>()
        {
            return this.I18N.TranslateEnumToDictionary<TEnum>(this.Name);
        }

        public List<string> TranslateEnumToList<TEnum>()
        {
            return this.I18N.TranslateEnumToList<TEnum>(this.Name);
        }

        public List<Tuple<TEnum, string>> TranslateEnumToTupleList<TEnum>()
        {
            return this.I18N.TranslateEnumToTupleList<TEnum>(this.Name);
        }

        private string BindSection(string key)
        {
            return $"{this.Name}.{key}";
        }
    }
}