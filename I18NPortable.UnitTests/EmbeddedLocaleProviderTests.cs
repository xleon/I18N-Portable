using System;
using System.Collections.Generic;
using System.Linq;
using I18NPortable.Providers;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class EmbeddedLocaleProviderTests
    {
        [Test]
        public void ResourceFolder_CanBe_Set()
        {
            var provider = new EmbeddedLocaleProvider(GetType().Assembly, "OtherLocales").Init();
            var locales = provider.GetAvailableLocales().ToList();

            Assert.Contains("es", locales);
            Assert.Contains("en", locales);
        }

        [Test]
        public void Locales_CannotBe_Duplicated()
        {
            var provider = new EmbeddedLocaleProvider(GetType().Assembly, "DuplicatedLocales");

            Assert.Throws<Exception>(() => provider.Init());
        }

        [Test]
        public void OnlyKnownFileExtensions_ShouldBe_Read()
        {
            var provider = new EmbeddedLocaleProvider(GetType().Assembly).Init();
            var locales = provider.GetAvailableLocales().ToList();

            Assert.IsFalse(locales.Contains("es-CO"));
        }

        [Test]
        public void KnownFileExtensions_Cannot_BeEmpty()
        {
            var provider = new EmbeddedLocaleProvider(GetType().Assembly, knownFileExtensions:new List<string>());

            Assert.Throws<Exception>(() => provider.Init());
        }

        [Test]
        public void AnyFileExtensions_CanBe_Loaded()
        {
            var knowExtensions = new[] {".weird1", ".weird2", ".weird3"};
            var provider = new EmbeddedLocaleProvider(GetType().Assembly, "OtherLocales", knowExtensions).Init();
            var locales = provider.GetAvailableLocales().ToList();

            Assert.Contains("fr", locales);
            Assert.Contains("ru", locales);
            Assert.Contains("pt", locales);
        }
    }
}
