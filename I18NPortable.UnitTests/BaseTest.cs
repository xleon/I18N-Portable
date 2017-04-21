using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public abstract class BaseTest
    {
        [SetUp]
        public void Init() =>
            I18N.Current = new I18N()
                .Init(GetType().Assembly);

        [TearDown]
        public void Finish() =>
            I18N.Current.Dispose();
    }
}
