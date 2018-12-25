using I18NPortable; //Todo re-import after release - imported from project not from NuGet repository 
using I18NPortable.CsvReader; //Todo re-import after release - imported from project not from NuGet repository
using I18NPortable.JsonReader; //Todo re-import after release - imported from project not from NuGet repository
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
        public void CsvLineReader_ShouldRead_CsvLocales()
        {
            I18N.Current = new I18N()
                .SetResourcesFolder("CsvLineLocales")
                .AddLocaleReader(new CsvLineReader(), ".csv")
                .Init(GetType().Assembly);

            I18N.Current.Locale = "es";

            Assert.AreEqual("uno", "one".Translate());
        }

        [Test]
        public void CsvColSingleFileReader_ShouldRead_CsvLocales()
        {
            I18N.Current = new I18N()
                .SetResourcesFolder("CsvColSingleFileLocales")
                .SingleFileResourcesMode()
                .AddSingleFileLocaleReader(new CsvColSingleFileReader(), ".csv")
                .Init(GetType().Assembly);

            I18N.Current.Locale = "es";

            Assert.AreEqual("uno", "one".Translate());
        }

        [Test]
        public void JsonKvpSingleFileReader_ShouldRead_CsvLocales()
        {
            I18N.Current = new I18N()
                .SetResourcesFolder("JsonKvpSingleFileLocales")
                .SingleFileResourcesMode()
                .AddSingleFileLocaleReader(new JsonKvpSingleFileReader(), ".json")
                .Init(GetType().Assembly);

            I18N.Current.Locale = "es";

            Assert.AreEqual("uno", "one".Translate());
        }
    }
}
