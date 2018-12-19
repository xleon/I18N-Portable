using I18NPortable;
using I18NPortable.CsvReader;
using I18NPortable.JsonReader;
using NUnit.Framework;

namespace NugetTest
{
    [TestFixture]
    public class NugetTests
    {
        [Test]
        public void JsonKvpReader_ShouldRead_JsonLocales()
        {
            I18N.Current = new I18N()
                .SetResourcesFolder("JsonKvpLocales")
                .AddLocaleReader(new JsonKvpReader(), ".json")
                .Init(GetType().Assembly);

            I18N.Current.Locale = "es";

            Assert.AreEqual("uno", "one".Translate());
        }

        [Test]
        public void JsonListReader_ShouldRead_JsonLocales()
        {
            I18N.Current = new I18N()
                .SetResourcesFolder("JsonListLocales")
                .AddLocaleReader(new JsonListReader(), ".json")
                .Init(GetType().Assembly);

            I18N.Current.Locale = "es";

            Assert.AreEqual("uno", "one".Translate());
        }

        [Test]
        public void CsvReader_ShouldRead_CsvLocales()
        {
            I18N.Current = new I18N()
                .SetResourcesFolder("CsvLineLocales")
                .AddLocaleReader(new CsvLineReader(), ".csv")
                .Init(GetType().Assembly);

            I18N.Current.Locale = "es";

            Assert.AreEqual("uno", "one".Translate());
        }
    }
}
