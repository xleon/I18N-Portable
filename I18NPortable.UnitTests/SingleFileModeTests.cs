using I18NPortable.CsvReader;
using I18NPortable.JsonReader;
using I18NPortable.Readers;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class SingleFileModeTests : BaseSingleFileTest
    {
        [Test]
        public void DefaultManifestAndResourcesName()
        {
            Assert.AreEqual(2, I18N.Current.Languages.Count);
        }

        [Test]
        public void CustomManifestAndResourcesName()
        {
            var current = new I18N()
                .SingleFileResourcesMode("customManifest.txt", "customResources.txt")
                .SetResourcesFolder("SingleFileLocales")
                .Init(GetType().Assembly);

            Assert.AreEqual(2, current.Languages.Count);
        }

        [Test]
        public void ManifestName_ShouldNot_BeInvalid()
        {
            Assert.Throws<I18NException>(() =>
            {
                new I18N()
                    .SingleFileResourcesMode("adfsdgasdg.txt", "customResources.txt")
                    .SetResourcesFolder("SingleFileLocales")
                    .Init(GetType().Assembly);
            });
        }

        [Test]
        public void ResourcesName_ShouldNot_BeInvalid()
        {
            Assert.Throws<I18NException>(() =>
            {
                new I18N()
                    .SingleFileResourcesMode("localeManifest.txt", "fsdfsdfsdf.txt")
                    .SetResourcesFolder("SingleFileLocales")
                    .Init(GetType().Assembly);
            });
        }

        [Test]
        public void ResourcesName_CanBe_WithoutExtension()
        {
            var current = new I18N()
                .SingleFileResourcesMode("localeManifest.txt", "resources")
                .SetResourcesFolder("SingleFileLocales")
                .Init(GetType().Assembly);

            Assert.AreEqual(2, current.Languages.Count);
        }

        [Test]
        public void OnlyLastSet_SingleFileReader_IsUsed()
        {
           new I18N()
               .SingleFileResourcesMode()
               .SetResourcesFolder("SingleFileLocales")
               .SetSingleFileLocaleReader(new CsvColSingleFileReader(), ".csv")
               .SetSingleFileLocaleReader(new JsonListSingleFileReader(), ".json")
               .SetSingleFileLocaleReader(new TextKvpSingleFileReader(), ".txt")
               .Init(GetType().Assembly);
        }
    }
}
