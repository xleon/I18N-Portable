using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace I18NPortable.UnitTests
{
    public enum Animals
    {
        Dog,
        Cat,
        Rat,
        Snake
    }

    public enum Fruit
    {
        Orange,
        Apple,
        Banana
    }

    public enum TestEnum
    {
        TestEnumValue1
    }

    public class I18NMock : II18N
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
        }

        public string this[string key]
        {
            get { throw new NotImplementedException(); }
        }

        public PortableLanguage Language { get; set; }
        public string Locale { get; set; }
        public List<PortableLanguage> Languages { get; }

        public II18N SetNotFoundSymbol(string symbol)
        {
            throw new NotImplementedException();
        }

        public II18N SetLogger(Action<string> output)
        {
            throw new NotImplementedException();
        }

        public II18N SetThrowWhenKeyNotFound(bool enabled)
        {
            throw new NotImplementedException();
        }

        public II18N SetFallbackLocale(string locale)
        {
            throw new NotImplementedException();
        }

        public II18N Init(Assembly hostAssembly)
        {
            throw new NotImplementedException();
        }

        public string GetDefaultLocale()
        {
            throw new NotImplementedException();
        }

        public string Translate(string key, params object[] args)
        {
            return "mocked translation";
        }

        public string TranslateOrNull(string key, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Dictionary<TEnum, string> TranslateEnumToDictionary<TEnum>()
        {
            throw new NotImplementedException();
        }

        public List<string> TranslateEnumToList<TEnum>()
        {
            throw new NotImplementedException();
        }

        public List<Tuple<TEnum, string>> TranslateEnumToTupleList<TEnum>()
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }
    }
}