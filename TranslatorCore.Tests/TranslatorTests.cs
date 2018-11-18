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
        public void Finish() =>
            Translator.Current?.Dispose();

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
        
        [Test]
        public void Overrides_can_be_used_without_setup()
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

        #region Binding

        [Test]
        public void Locale_property_is_bindable()
        {
            Translator.Current.Locale = "es";
            var locale = Translator.Current.Locale;

            Translator.Current.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("Locale"))
                    locale = ((ITranslator)sender).Locale;
            };

            Translator.Current.Locale = "en";

            Assert.AreEqual("en", locale);

            Translator.Current.Locale = "es";

            Assert.AreEqual("es", locale);
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

        #region Locales

        [Test]
        public void Culture_2_letter_isocode_is_used_when_it_matches_supported_locale()
        {
            Helpers.SetCulture("en-GB");
            Translator.Current.Setup(s => s.SetSupportedLocales("es", "en"));
            Assert.AreEqual("en", Translator.Current.Locale);
        }

        [Test]
        public void When_no_locale_matches_then_first_in_the_list_is_taken()
        {
            Helpers.SetCulture("pt");
            Translator.Current.Setup(s => s.SetSupportedLocales("es", "en"));
            Assert.AreEqual("es", Translator.Current.Locale);
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