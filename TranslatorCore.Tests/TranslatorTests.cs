using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace TranslatorCore.Tests
{
    public class TranslatorTests
    {
        [TearDown]
        public void Finish()
        {
            Translator.Current?.Dispose();
            Helpers.SetCulture("en");
        }

        #region Misc

        [Test]
        public void Translator_logs_to_an_action()
        {
            var logs = new List<string>();
            void Logger(string text) => logs.Add(text);

            Translator.Current.Setup(s => s.SetLogger(Logger));
            
            Assert.IsTrue(logs.Count > 0);
        }
        
        [Test]
        public void New_instance_should_be_created_after_disposing()
        {
            var translator = Translator.Current;
            translator.Dispose();

            Assert.AreNotEqual(translator, Translator.Current);
        }

        #endregion

        #region Translation overrides

        [Test]
        public void Translation_overrides_work_without_setup()
        {
            Translator.Current.SetOverrides(new Dictionary<string, string>
            {
                { "key", "value" }
            });

            Translator.Current.AddOverrides(new Dictionary<string, string>
            {
                {"key2", "value2"}
            });
            
            Translator.Current["key3"] = "value3";
            
            Assert.AreEqual("value", Translator.Current.Translate("key"));
            Assert.AreEqual("value", Translator.Current.TranslateFrom("key", "res"));
            Assert.AreEqual("value2", Translator.Current.Translate("key2"));
            Assert.AreEqual("value2", Translator.Current.TranslateFrom("key2", "res"));
            Assert.AreEqual("value3", Translator.Current.Translate("key3"));
            Assert.AreEqual("value3", Translator.Current.TranslateFrom("key3", "res"));
        }

        #endregion

        #region Exceptions

        [Test]
        public void Accessing_culture_code_without_setup_or_supported_locales_should_throw()
        {
            Assert.Throws<TranslatorException>(() =>  { var code = Translator.Current.CultureCode; });

            Translator.Current.Setup(s => s);
            
            Assert.Throws<TranslatorException>(() => {  var code = Translator.Current.CultureCode; });
        }

        [Test]
        public void Translating_with_bad_setup_should_throw()
        {
            Assert.Throws<TranslatorException>(() => Translator.Current.Translate("key"));
            Assert.Throws<TranslatorException>(() => Translator.Current
                .Setup(s => s).Translate("key"));
        }

        #endregion
        
        #region Binding

        [Test]
        public void CultureCode_property_is_bindable()
        {
            string locale = null;

            Translator.Current.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(Translator.CultureCode)))
                    locale = ((ITranslator)sender).CultureCode;
            };
            
            Helpers.SetCulture("es");
            var l = Translator.Current.Setup(s => s.SetSupportedLocales("es", "en")).CultureCode;
            
            Assert.AreEqual("es", locale);
            Assert.AreEqual("es", l);

            Translator.Current.CultureCode = "en";
            Assert.AreEqual("en", locale);
        }

        [Test]
        public void Language_property_is_bindable()
        {
            throw new NotImplementedException();
        }
        
        [Test]
        public void Translation_overrides_trigger_indexer_bindings()
        {
            string translation = null;

            Translator.Current.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("Item[]"))
                    translation = Translator.Current.Translate("key");
            };
            
            Translator.Current.SetOverrides(new Dictionary<string, string> { {"key", "value"} });
            Assert.AreEqual("value", translation);
            
            Translator.Current.AddOverrides(new Dictionary<string, string> { {"key", "value2"} });
            Assert.AreEqual("value2", translation);

            Translator.Current["key"] = "value3";
            Assert.AreEqual("value3", translation);
        }

        #endregion

        #region Culture

        [Test]
        public void Culture_2letter_isocode_is_used_when_culture_name_not_supported()
        {
            Helpers.SetCulture("en-GB"); // culture name
            Translator.Current.Setup(s => s.SetSupportedLocales("es", "en")); // only 2-letter iso codes
            Assert.AreEqual("en", Translator.Current.CultureCode);
        }

        [Test]
        public void Culture_name_has_preference_over_iso_codes()
        {
            Helpers.SetCulture("es-MX");
            Translator.Current.Setup(s => s.SetSupportedLocales("es", "es-ES", "es-MX"));
            Assert.AreEqual("es-MX", Translator.Current.CultureCode);
        }

        [Test]
        public void When_current_culture_not_supported_then_first_supported_culture_is_taken()
        {
            Helpers.SetCulture("pt");
            Translator.Current.Setup(s => s.SetSupportedLocales("es", "en"));
            Assert.AreEqual("es", Translator.Current.CultureCode);
        }

        #endregion
    }

    public static class Helpers
    {
        public static Mock<ILocaleLoader> GetLoaderMock()
        {
            var mock = new Mock<ILocaleLoader>();
            
            mock
                .Setup(loader => loader.Load("es", null))
                .Returns(new Dictionary<string, string>
                {
                    {"one", "uno"}
                });
            
            mock
                .Setup(loader => loader.Load("en", null))
                .Returns(new Dictionary<string, string>
                {
                    {"one", "one"}
                });
            
            mock
                .Setup(loader => loader.Load("es", "screen1"))
                .Returns(new Dictionary<string, string>
                {
                    {"two", "dos"}
                });
            
            mock
                .Setup(loader => loader.Load("en", "screen1"))
                .Returns(new Dictionary<string, string>
                {
                    {"two", "two"}
                });

            return mock;
        }
        
        public static void SetCulture(string cultureName)
        {
            CultureInfo.DefaultThreadCurrentCulture =
                CultureInfo.DefaultThreadCurrentUICulture =
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);
        }
    }
}