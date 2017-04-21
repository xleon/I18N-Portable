using System.Collections.Generic;
using I18NPortable.UnitTests.Util;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class I18NTests : BaseTest
    {
        [Test]
        public void Logger_CanBeSet_AsAction()
        {
            var logs = new List<string>();
            void Logger(string text) => logs.Add(text);

            I18N.Current.SetLogger(Logger);
            I18N.Current.Locale = "en";
            I18N.Current.Locale = "es";

            Assert.IsTrue(logs.Count > 0);
        }

        [Test]
        public void I18N_CanBeMocked()
        {
            var mock = new I18NMock();
            I18N.Current = mock;

            Assert.AreEqual("mocked translation", I18N.Current.Translate("something"));
        }

        [Test]
        public void I18N_CanBeDisposed()
        {
            I18N.Current.PropertyChanged += (sender, args) => { };
            I18N.Current.Dispose();

            Assert.IsNull(I18N.Current.Language);
            Assert.IsNull(I18N.Current.Languages);
            Assert.IsNull(I18N.Current.Locale);
        }
    }
}