using I18NPortable.JsonReader;
using I18NPortable.Readers;
using NUnit.Framework;
using TestHostAssembly;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class SetSingleFileLocaleReaderTests : BaseSingleFileTest
    {
        [Test]
        public void Reader_ShouldNot_BeNull()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.SetSingleFileLocaleReader(null, ".txt");
            });
        }

        [Test]
        public void ReaderExtension_ShouldNot_BeNullOrEmpty()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.SetSingleFileLocaleReader(new TextKvpSingleFileReader(), null);
            });

            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.SetSingleFileLocaleReader(new TextKvpSingleFileReader(), string.Empty);
            });
        }

        [Test]
        public void ReaderExtension_Should_StartWithDot()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.SetSingleFileLocaleReader(new TextKvpSingleFileReader(), "json");
            });
        }

        [Test]
        public void ReaderExtension_Should_ContainAtLeastOneChar()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.SetSingleFileLocaleReader(new TextKvpSingleFileReader(), ".");
            });
        }

        [Test]
        public void ReaderExtension_Should_ContainJustOneDot()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.SetSingleFileLocaleReader(new TextKvpSingleFileReader(), "..json");
            });
        }

        [Test]
        public void UnknownResourceExtension_Should_BeIgnored()
        {
            var current = new I18N()
                .SingleFileResourcesMode()
                .SetResourcesFolder("SingleFileLocales")
                .SetThrowWhenKeyNotFound(true)
                .SetSingleFileLocaleReader(new TextKvpSingleFileReader(), ".txt22222")
                .Init(typeof(TestHostAssemblyDummy).Assembly);

            Assert.AreEqual(2, current.Languages.Count);
        }
    }
}
