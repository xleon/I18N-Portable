using System;
using System.Collections.Generic;
using I18NPortable.UnitTests.Util;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class TranslationTests : BaseTest
    {
        [Test]
        public void Keys_ShouldBe_Translated()
        {
            I18N.Current.Locale = "en";
            Assert.AreEqual("one", I18N.Current.Translate("one"));
            Assert.AreEqual("two", I18N.Current.Translate("two"));
            Assert.AreEqual("three", I18N.Current.Translate("three"));

            I18N.Current.Locale = "es";
            Assert.AreEqual("uno", I18N.Current.Translate("one"));
            Assert.AreEqual("dos", I18N.Current.Translate("two"));
            Assert.AreEqual("tres", I18N.Current.Translate("three"));
        }

        [Test]
        public void Indexer_Should_TranslateKey()
        {
            I18N.Current.Locale = "en";
            Assert.AreEqual("one", I18N.Current["one"]);

            I18N.Current.Locale = "es";
            Assert.AreEqual("uno", I18N.Current["one"]);
        }

        [Test]
        public void Translate_Should_FormatString()
        {
            I18N.Current.Locale = "en";
            Assert.AreEqual("Hello Marta, you´ve got 56 emails",
                I18N.Current.Translate("Mailbox.Notification", "Marta", 56));

            I18N.Current.Locale = "es";
            Assert.AreEqual("Hola David, tienes 47 emails",
                I18N.Current.Translate("Mailbox.Notification", "David", 47));
        }

        [Test]
        public void NotFoundSymbol_CanBe_Changed()
        {
            I18N.Current.SetNotFoundSymbol("$$");
            var nonExistent = I18N.Current.Translate("nonExistentKey");

            Assert.AreEqual("$$nonExistentKey$$", nonExistent);
        }

        [Test]
        public void TranslateOrNull_ShouldReturn_Null_WhenKeyIsNotFound()
        {
            Assert.IsNull(I18N.Current.TranslateOrNull("nonExistentKey"));
            Assert.IsNull(I18N.Current.TranslateOrNull("nonExistentKey", "one", "two"));
        }

        [Test]
        public void TranslateOrNull_Should_Translate()
        {
            I18N.Current.Locale = "es";
            Assert.AreEqual("uno", I18N.Current.TranslateOrNull("one"));
            Assert.AreEqual("Hola Diego, tienes 3 emails",
                I18N.Current.TranslateOrNull("Mailbox.Notification", "Diego", "3"));
        }

        [Test]
        public void WillThrow_WhenKeyNotFound_AndSetupToDoSo()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                I18N.Current.SetThrowWhenKeyNotFound(true);
                I18N.Current.Translate("fake");
            });
        }

        [Test]
        public void NotFoundSymbol_ShouNot_BeNullOrEmpty()
        {
            I18N.Current.SetNotFoundSymbol("##");
            I18N.Current.SetNotFoundSymbol(null);

            Assert.AreEqual("##missing##", "missing".Translate());

            I18N.Current.SetNotFoundSymbol(string.Empty);

            Assert.AreEqual("##missing##", "missing".Translate());
        }

        [Test]
        public void Translation_ShouldConsider_LineBreakCharacters()
        {
            I18N.Current.Locale = "en";

            var textWithLineBreaks = I18N.Current.Translate("TextWithLineBreakCharacters");
            var textWithLineBreaksOrNull = I18N.Current.Translate("TextWithLineBreakCharacters");

            var expected = $"Line One{Environment.NewLine}Line Two{Environment.NewLine}Line Three";

            Assert.AreEqual(expected, textWithLineBreaks);
            Assert.AreEqual(expected, textWithLineBreaksOrNull);
        }

        [Test]
        public void EnumTranslation_ShouldConsider_LineBreakCharacters()
        {
            I18N.Current.Locale = "en";

            var animals = I18N.Current.TranslateEnumToDictionary<Animals>();

            Assert.AreEqual($"Good{Environment.NewLine}Snake", animals[Animals.Snake]);
        }

        [Test]
        public void TranslationValue_Supports_Multiline()
        {
            I18N.Current.Locale = "en";
            var multilineValue = I18N.Current.Translate("Multiline");
            var expected = $"Line One{Environment.NewLine}Line Two{Environment.NewLine}Line Three";

            Assert.AreEqual(expected, multilineValue);

            I18N.Current.Locale = "es";
            multilineValue = I18N.Current.Translate("Multiline");
            expected = $"Línea Uno{Environment.NewLine}Línea Dos{Environment.NewLine}Línea Tres";

            Assert.AreEqual(expected, multilineValue);
        }

        [Test]
        public void Enum_CanBeTranslated_ToStringList()
        {
            I18N.Current.Locale = "es";
            var list = I18N.Current.TranslateEnumToList<Animals>();

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("Perro", list[0]);
            Assert.AreEqual("Gato", list[1]);
            Assert.AreEqual("Rata", list[2]);
        }

        [Test]
        public void Enum_CanBeTranslated_ToDictionary()
        {
            I18N.Current.Locale = "en";
            var animals = I18N.Current.TranslateEnumToDictionary<Animals>();

            Assert.AreEqual("Dog", animals[Animals.Dog]);
            Assert.AreEqual("Cat", animals[Animals.Cat]);
            Assert.AreEqual("Rat", animals[Animals.Rat]);

            I18N.Current.Locale = "es";
            animals = I18N.Current.TranslateEnumToDictionary<Animals>();

            Assert.AreEqual("Perro", animals[Animals.Dog]);
            Assert.AreEqual("Gato", animals[Animals.Cat]);
            Assert.AreEqual("Rata", animals[Animals.Rat]);
        }

        [Test]
        public void Enum_CanBeTranslated_ToTupleList()
        {
            I18N.Current.Locale = "en";
            var animalsTupleList = I18N.Current.TranslateEnumToTupleList<Animals>();

            Assert.AreEqual(4, animalsTupleList.Count);
            Assert.AreEqual("Dog", animalsTupleList[0].Item2);
            Assert.AreEqual(animalsTupleList[0].Item1, Animals.Dog);
            Assert.AreEqual("Rat", animalsTupleList[2].Item2);
            Assert.AreEqual(animalsTupleList[2].Item1, Animals.Rat);
        }
    }
}
