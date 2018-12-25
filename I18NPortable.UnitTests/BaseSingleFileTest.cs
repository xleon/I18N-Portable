using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public abstract class BaseSingleFileTest : BaseTest
    {
        [SetUp]
        public new void Init() =>
            I18N.Current = new I18N()
                .SingleFileResourcesMode()
                .SetResourcesFolder("SingleFileLocales")
                .Init(GetType().Assembly);
    }
}
