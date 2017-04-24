using System.Collections.Generic;
using System.Linq;
using I18NPortable.JsonReader;
using I18NPortable.Readers;
using I18NPortable.UnitTests.Util;
using NUnit.Framework;
using TestHostAssembly;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class LocaleLoadingTests : BaseTest
    {
        [Test]
        public void EmbbededLocales_ShouldBe_Discovered()
        {
            var languages = I18N.Current.Languages;
            Assert.IsTrue(languages.Count > 2);
        }

        [Test]
        public void Init_ShouldThrow_If_NoLocalesAvailable()
        {
            var assemblyWithoutLocales = I18N.Current.GetType().Assembly;

            Assert.Throws<I18NException>(() => I18N.Current.Init(assemblyWithoutLocales));
        }

        [Test]
        public void TryingToLoad_CurrentLanguage_WillDoNothing()
        {
            I18N.Current.Locale = "en";
            I18N.Current.Locale = "es";

            var logs = new List<string>();
            void Logger(string text) => logs.Add(text);
            I18N.Current.SetLogger(Logger);
            I18N.Current.Locale = "es";

            Assert.AreEqual(1, logs.Count);
        }

        [Test]
        public void CorrectLocale_IsLoaded_WhenSettingLanguage()
        {
            var languages = I18N.Current.Languages;
            var es = languages.FirstOrDefault(x => x.Locale.Equals("es"));
            var en = languages.FirstOrDefault(x => x.Locale.Equals("en"));

            I18N.Current.Language = es;
            Assert.AreEqual("es", I18N.Current.Locale);

            I18N.Current.Language = en;
            Assert.AreEqual("en", I18N.Current.Locale);
        }

        [Test]
        public void CurrentLanguage_Matches_LoadedLocale()
        {
            I18N.Current.Locale = "en";
            Assert.AreEqual("en", I18N.Current.Language.Locale);

            I18N.Current.Locale = "es";
            Assert.AreEqual("es", I18N.Current.Language.Locale);
        }

        [Test]
        public void LoadLanguage_ShouldLoad_MatchingLocale()
        {
            var language = new PortableLanguage { Locale = "en", DisplayName = "English" };
            I18N.Current.Language = language;

            Assert.AreEqual("one", I18N.Current.Translate("one"));

            language = new PortableLanguage { Locale = "es", DisplayName = "Español" };
            I18N.Current.Language = language;

            Assert.AreEqual("uno", I18N.Current.Translate("one"));
        }

        [Test]
        public void LoadingNonExistentLocale_ShouldThrow()
        {
            Assert.Throws<I18NException>(() => I18N.Current.Locale = "fr");
        }

        [Test]
        public void FallbackLocale_ShouldBeLoaded_When_RequestedLocaleIsNotAvailable()
        {
            Helpers.SetCulture("pt-PT");

            I18N.Current.SetFallbackLocale("en").Init(GetType().Assembly);

            Assert.AreEqual("en", I18N.Current.Locale);
        }

        [Test]
        public void FallbackLocale_ShouldBeIgnored_IfNotAvailable()
        {
            Helpers.SetCulture("pt-PT");

            I18N.Current.SetFallbackLocale("fr").Init(GetType().Assembly);

            Assert.AreEqual("en", I18N.Current.Locale);
        }

        [Test]
        public void DefaultLocale_ShouldBeNull_WhenSystemLocaleIsNotSupported()
        {
            I18N.Current.Dispose();

            Helpers.SetCulture("pt-PT");

            I18N.Current = new I18N().Init(GetType().Assembly);

            Assert.IsNull(I18N.Current.GetDefaultLocale());
        }

        [Test]
        public void ResourceFolder_CanBe_Set()
        {
            var current = new I18N();

            current
                .SetResourcesFolder("OtherLocales")
                .Init(GetType().Assembly);

            current.Locale = "es";
            Assert.AreEqual("bien", current.Translate("well"));

            current.Locale = "en";
            Assert.AreEqual("well", current.Translate("well"));
        }

        [Test]
        public void ResourceFolder_CanBe_Changed()
        {
            var current = I18N.Current;

            current
                .SetResourcesFolder("OtherLocales")
                .Init(GetType().Assembly);

            current.Locale = "es";
            Assert.AreEqual("bien", current.Translate("well"));

            current.Locale = "en";
            Assert.AreEqual("well", current.Translate("well"));
        }

        [Test]
        public void ResourcesFromOtherAssemblies_CanBe_Loaded()
        {
            var assembly = typeof(TestHostAssemblyDummy).Assembly;
            var current = new I18N().Init(assembly);

            current.Locale = "es";
            Assert.AreEqual("ensamblado anfitrión", current.Translate("host"));

            current.Locale = "en";
            Assert.AreEqual("host assembly", current.Translate("host"));
        }

        [Test]
        public void AnyFileExtension_CanBe_Loaded()
        {
            var current = new I18N()
                .AddLocaleReader(new TextKvpReader(), ".weird1")
                .AddLocaleReader(new TextKvpReader(), ".weird2")
                .AddLocaleReader(new TextKvpReader(), ".weird3")
                .SetResourcesFolder("OtherLocales")
                .Init(GetType().Assembly);

            var locales = current.Languages.Select(x => x.Locale).ToList();

            Assert.AreEqual(5, locales.Count);
            Assert.Contains("fr", locales);
            Assert.Contains("ru", locales);
            Assert.Contains("pt", locales);
        }

        [Test]
        public void Locales_CannotBe_Duplicated()
        {
            var current = new I18N()
                .SetResourcesFolder("DuplicatedLocales")
                .AddLocaleReader(new JsonKvpReader(), ".json");

            Assert.Throws<I18NException>(() => current.Init(GetType().Assembly));
        }

        [Test]
        public void OnlyKnownFileExtensions_ShouldBe_Read()
        {
            I18N.Current
                .SetResourcesFolder("OtherLocales")
                .Init(GetType().Assembly);

            var locales = I18N.Current.Languages.Select(x => x.Locale).ToList();

            Assert.AreEqual(2, locales.Count);
            Assert.Contains("en", locales);
            Assert.Contains("es", locales);

            I18N.Current
                .AddLocaleReader(new TextKvpReader(), ".weird1")
                .Init(GetType().Assembly);

            locales = I18N.Current.Languages.Select(x => x.Locale).ToList();

            Assert.AreEqual(3, locales.Count);
            Assert.Contains("fr", locales);
        }
    }
}
