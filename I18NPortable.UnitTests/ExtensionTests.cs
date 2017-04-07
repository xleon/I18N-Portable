using System;
using System.Reflection;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class ExtensionTests
    {
        [SetUp]
        public void Init() =>
            I18N.Current = new I18N()
                .SetNotFoundSymbol("?")
                .SetThrowWhenKeyNotFound(false)
                .Init(GetType().GetTypeInfo().Assembly);

        [TearDown]
        public void CanTranslate_WithStringExtensionMethod()
        {
            I18N.Current.Locale = "en";
            Assert.AreEqual("one", "one".Translate());

            I18N.Current.Locale = "es";
            Assert.AreEqual("uno", "one".Translate());
        }

        [Test]
        public void TranslateOrNullExtension_ShouldReturn_Null_WhenKeyIsNotFound()
        {
            Assert.IsNull("nonExistentKey".TranslateOrNull());
        }

        [Test]
        public void TranslateOrNullExtension_ShouldTranslateKeys()
        {
            I18N.Current.Locale = "es";
            Assert.AreEqual("uno", "one".TranslateOrNull());
        }

        [Test]
        public void UnescapeLineBreaks_ShouldWork()
        {
            const string sample = "Hello\\r\\nfrom\\nthe other side";
            var unescaped = sample.UnescapeLineBreaks();
            var expected = $"Hello{Environment.NewLine}from{Environment.NewLine}the other side";

            Assert.AreEqual(expected, unescaped);
        }

        [Test]
        public void CapitalizeFirstLetter_ShouldWork()
        {
            Assert.AreEqual("English", "english".CapitalizeFirstCharacter());
            Assert.AreEqual("E", "e".CapitalizeFirstCharacter());
            Assert.AreEqual(" ", " ".CapitalizeFirstCharacter());

            string nullString = null;
            Assert.IsNull(nullString.CapitalizeFirstCharacter());
        }

        [Test]
        public void EveryEnum_CanBeTranslated_WithExtension()
        {
            I18N.Current.Locale = "en";

            var apple = Fruit.Apple.Translate();
            var orange = Fruit.Orange.Translate();
            var banana = Fruit.Banana.Translate();

            Assert.AreEqual("big apple", apple);
            Assert.AreEqual("great orange", orange);
            Assert.AreEqual("nice banana", banana);
        }

        [Test]
        public void NonLocalizedEnums_CanBeTranslated()
        {
            Assert.AreEqual("?TestEnum.TestEnumValue1?", TestEnum.TestEnumValue1.Translate());
        }
    }
}