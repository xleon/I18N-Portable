using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace I18NPortable.Tests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestInitialize]
        public void Init() =>
            I18N.Current
                .SetNotFoundSymbol("?")
                .SetThrowWhenKeyNotFound(false)
                .Init(GetType().GetTypeInfo().Assembly);

        [TestMethod]
        public void CanTranslate_WithStringExtensionMethod()
        {
            I18N.Current.Locale = "en";
            Assert.AreEqual("one", "one".Translate());

            I18N.Current.Locale = "es";
            Assert.AreEqual("uno", "one".Translate());
        }

        [TestMethod]
        public void TranslateOrNullExtension_ShouldReturn_Null_WhenKeyIsNotFound()
        {
            Assert.IsNull("nonExistentKey".TranslateOrNull());
        }

        [TestMethod]
        public void TranslateOrNullExtension_ShouldTranslateKeys()
        {
            I18N.Current.Locale = "es";
            Assert.AreEqual("uno", "one".TranslateOrNull());
        }

        [TestMethod]
        public void UnescapeLineBreaks_ShouldWork()
        {
            const string sample = "Hello\\r\\nfrom\\nthe other side";
            var unescaped = sample.UnescapeLineBreaks();
            var expected = $"Hello{Environment.NewLine}from{Environment.NewLine}the other side";

            Assert.AreEqual(expected, unescaped);
        }

        [TestMethod]
        public void CapitalizeFirstLetter_ShouldWork()
        {
            Assert.AreEqual("English", "english".CapitalizeFirstCharacter());
            Assert.AreEqual("E", "e".CapitalizeFirstCharacter());
            Assert.AreEqual(" ", " ".CapitalizeFirstCharacter());

            string nullString = null;
            Assert.IsNull(nullString.CapitalizeFirstCharacter());
        }

        [TestMethod]
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

        [TestMethod]
        public void NonTranslatedExtensions_CanStillBeTranslated()
        {
            Assert.AreEqual("?TestEnum.TestEnumValue1?", TestEnum.TestEnumValue1.Translate());
        }
    }

    public enum Fruit
    {
        Orange,
        Apple,
        Banana
    }

    public enum TestEnum
    {
        TestEnumValue1
    }
}
