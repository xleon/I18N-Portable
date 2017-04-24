using System.Linq;
using I18NPortable.UnitTests.Util;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class CultureTests : BaseTest
    {
        [TestCase("es", "Español")]
        [TestCase("en", "English")]
        [TestCase("pt-BR", "Português (Brasil)")]
        [TestCase("es-MX", "Español (México)")]
        public void Languages_ShouldHave_CorrectDisplayName(string locale, string displayName)
        {
            var languages = I18N.Current.Languages;
            var language = languages.FirstOrDefault(x => x.Locale.Equals(locale));

            Assert.AreEqual(displayName, language?.DisplayName);
        }

        [TestCase("es-MX")]
        [TestCase("pt-BR")]
        public void LocalesWithWholeCultureNames_CanBeDefault(string cultureName)
        {
            Helpers.SetCulture(cultureName);
            I18N.Current = new I18N().Init(GetType().Assembly);

            Assert.AreEqual(cultureName, I18N.Current.GetDefaultLocale());
        }

        [TestCase("es-MX", "Fruit.Banana", "banana")]
        [TestCase("es-ES", "Fruit.Banana", "plátano")]
        [TestCase("pt-BR", "hello", "oi")]
        public void LocaleWithWholeCultureNames_GetLoaded_AsDefault(string cultureName, string key, string translation)
        {
            Helpers.SetCulture(cultureName);
            I18N.Current = new I18N().Init(GetType().Assembly);

            Assert.AreEqual(translation, I18N.Current.Translate(key));
        }
    }
}
