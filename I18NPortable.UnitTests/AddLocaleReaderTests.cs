using System;
using I18NPortable.Readers;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class AddLocaleReaderTests
    {
        [TearDown]
        public void Finish() =>
            I18N.Current.Dispose();

        [Test]
        public void Reader_ShouldNot_BeNull()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                I18N.Current.AddLocaleReader(null, ".txt");
            });
        }

        [Test]
        public void ReaderExtension_ShouldNot_BeNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), null);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), string.Empty);
            });
        }

        [Test]
        public void ReaderExtension_Should_StartWithDot()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), "json");
            });
        }

        [Test]
        public void ReaderExtension_Should_ContainAtLeastOneChar()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), ".");
            });
        }

        [Test]
        public void ReaderExtension_Should_ContainJustOneDot()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                I18N.Current.AddLocaleReader(new TextKvpReader(), "..json");
            });
        }

        [Test]
        public void SameReader_CannotBeAdded_Twice()
        {
            var reader = new TextKvpReader();

            Assert.Throws<ArgumentException>(() =>
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

            Assert.Throws<ArgumentException>(() =>
            {
                I18N.Current
                    .AddLocaleReader(reader, ".txt")
                    .AddLocaleReader(reader2, ".txt");
            });
        }
    }
}
