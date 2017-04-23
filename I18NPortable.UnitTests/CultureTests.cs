using System.Linq;
using I18NPortable.UnitTests.Util;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class CultureTests : BaseTest
    {
        [Test]
        public void Languages_ShouldHave_CorrectDisplayName()
        {
            var languages = I18N.Current.Languages;
            var es = languages.FirstOrDefault(x => x.Locale.Equals("es"));
            var en = languages.FirstOrDefault(x => x.Locale.Equals("en"));

            Assert.AreEqual("English", en?.DisplayName);
            Assert.AreEqual("Español", es?.ToString());
        }

        [Test]
        public void LocalesWithWholeCultureNames_CanBeDefault()
        {
            Helpers.SetCulture("es-MX");

            I18N.Current = new I18N().Init(GetType().Assembly);

            Assert.AreEqual("es-MX", I18N.Current.GetDefaultLocale());

            Helpers.SetCulture("pt-BR");

            I18N.Current = new I18N().Init(GetType().Assembly);

            Assert.AreEqual("pt-BR", I18N.Current.GetDefaultLocale());
            Assert.AreEqual("oi", "hello".Translate());
        }

        [Test]
        public void LocaleWithWholeCultureNames_GetLoaded_AsDefault()
        {
            Helpers.SetCulture("es-MX");
            I18N.Current = new I18N().Init(GetType().Assembly);

            Assert.AreEqual("banana", I18N.Current.Translate("Fruit.Banana"));

            Helpers.SetCulture("es-ES");
            I18N.Current = new I18N().Init(GetType().Assembly);

            Assert.AreEqual("plátano", I18N.Current.Translate("Fruit.Banana"));

            Helpers.SetCulture("pt-BR");
            I18N.Current = new I18N().Init(GetType().Assembly);

            Assert.AreEqual("oi", "hello".Translate());
        }
    }
}
