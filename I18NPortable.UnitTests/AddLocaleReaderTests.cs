using System.Reflection;
using I18NPortable.Readers;
using NUnit.Framework;
using TestHostAssembly;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class AddLocaleReaderTests : BaseTest
    {
        [Test]
        public void Reader_ShouldNot_BeNull()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.AddLocaleReader(null, ".txt");
            });
        }

        [Test]
        public void ReaderExtension_ShouldNot_BeNullOrEmpty()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), null);
            });

            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), string.Empty);
            });
        }

        [Test]
        public void ReaderExtension_Should_StartWithDot()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), "json");
            });
        }

        [Test]
        public void ReaderExtension_Should_ContainAtLeastOneChar()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), ".");
            });
        }

        [Test]
        public void ReaderExtension_Should_ContainJustOneDot()
        {
            Assert.Throws<I18NException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), "..json");
            });
        }

        [Test]
        public void SameReader_CannotBeAdded_Twice()
        {
            var reader = new TextKvpReader();

            Assert.Throws<I18NException>(() =>
            {
                I18N.Current
                    .AddLocaleReader(reader, ".txt")
                    .AddLocaleReader(reader, ".txt");
            });
        }

        [Test]
        public void SameExtension_CannotBeSet_ForDifferentReaders()
        {
            var reader = new TextKvpReader();
            var reader2 = new JsonKvpReader();

            Assert.Throws<I18NException>(() =>
            {
                I18N.Current
                    .AddLocaleReader(reader, ".txt")
                    .AddLocaleReader(reader2, ".txt");
            });
        }

        [Test]
        public void UnknownResourceExtension_Should_BeIgnored()
        {
            var current = new I18N()
                .SetThrowWhenKeyNotFound(true)
                .AddLocaleReader(new TextKvpReader(), ".txt22222")
                .Init(typeof(TestHostAssemblyDummy).Assembly);

            Assert.AreEqual(2, current.Languages.Count);
        }
    }
}
