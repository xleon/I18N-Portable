using System;
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
            throw new NotImplementedException();
        }

        [Test]
        public void SameLocale_Cannot_BeAddedTwice()
        {
            var provider = new EmbeddedLocaleProvider(GetType().Assembly, "BadLocales");

            Assert.Throws<Exception>(() => provider.Init());
        }

        [Test]
        public void OnlyKnownFileExtensions_ShouldBe_Read()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void KnownFileExtensions_Cannot_BeEmpty()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TxtExtension_WillBeUsed_When_KnownFileExtensions_IsNull()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AnyFileExtensions_CanBe_Loaded()
        {
            throw new NotImplementedException();
        }
    }
}
