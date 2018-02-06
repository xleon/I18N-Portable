using System;
using System.Collections.Generic;
using I18NPortable.JsonReader;
using I18NPortable.UnitTests.Util;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class TranslationTests : BaseTest
    {
        [TestCase("en", "one", "one")]
        [TestCase("en", "two", "two")]
        [TestCase("en", "three", "three")]
        [TestCase("en", "Animals", "List of animals")]
        [TestCase("en", "Animals.Dog", "Dog")]
        [TestCase("en", "Animals.Cat", "Cat")]
        [TestCase("en", "Animals.Rat", "Rat")]
        [TestCase("en", "Fruit.Orange", "great orange")]
        [TestCase("en", "Fruit.Apple", "big apple")]

        [TestCase("es", "one", "uno")]
        [TestCase("es", "two", "dos")]
        [TestCase("es", "three", "tres")]
        [TestCase("es", "Animals", "Lista de animales")]
        [TestCase("es", "Animals.Dog", "Perro")]
        [TestCase("es", "Animals.Cat", "Gato")]
        [TestCase("es", "Animals.Rat", "Rata")]
        [TestCase("es", "Fruit.Orange", "gran naranja")]
        [TestCase("es", "Fruit.Apple", "manzana grande")]
        public void Keys_ShouldBe_Translated(string locale, string key, string translation)
        {
            I18N.Current.Locale = locale;

            Assert.AreEqual(translation, I18N.Current.Translate(key));
            Assert.AreEqual(translation, I18N.Current[key]);
            Assert.AreEqual(translation, key.Translate());
            Assert.AreEqual(translation, key.TranslateOrNull());

            I18N.Current = new I18N()
                .SetResourcesFolder("JsonKvpLocales")
                .AddLocaleReader(new JsonKvpReader(), ".json")
                .Init(GetType().Assembly);

            I18N.Current.Locale = locale;

            Assert.AreEqual(translation, I18N.Current.Translate(key));
            Assert.AreEqual(translation, I18N.Current[key]);
            Assert.AreEqual(translation, key.Translate());
            Assert.AreEqual(translation, key.TranslateOrNull());

            I18N.Current = new I18N()
                .SetResourcesFolder("JsonListLocales")
                .AddLocaleReader(new JsonListReader(), ".json")
                .Init(GetType().Assembly);

            I18N.Current.Locale = locale;

            Assert.AreEqual(translation, I18N.Current.Translate(key));
            Assert.AreEqual(translation, I18N.Current[key]);
            Assert.AreEqual(translation, key.Translate());
            Assert.AreEqual(translation, key.TranslateOrNull());
        }

        [TestCase("en", "Mailbox.Notification", "Hello Marta, you´ve got 56 emails")]
        [TestCase("es", "Mailbox.Notification", "Hola Marta, tienes 56 emails")]
        public void Translate_Should_FormatString(string locale, string key, string translation)
        {
            I18N.Current.Locale = locale;

            Assert.AreEqual(translation, I18N.Current.Translate(key, "Marta", 56));
            Assert.AreEqual(translation, key.Translate("Marta", 56));

            I18N.Current = new I18N()
                .SetResourcesFolder("JsonKvpLocales")
                .AddLocaleReader(new JsonKvpReader(), ".json")
                .Init(GetType().Assembly);

            I18N.Current.Locale = locale;

            Assert.AreEqual(translation, I18N.Current.Translate(key, "Marta", 56));
            Assert.AreEqual(translation, key.Translate("Marta", 56));

            I18N.Current = new I18N()
                .SetResourcesFolder("JsonListLocales")
                .AddLocaleReader(new JsonListReader(), ".json")
                .Init(GetType().Assembly);

            I18N.Current.Locale = locale;

            Assert.AreEqual(translation, I18N.Current.Translate(key, "Marta", 56));
            Assert.AreEqual(translation, key.Translate("Marta", 56));
        }

        [TestCase("en", "Line One", "Line Two", "Line Three")]
        [TestCase("es", "Línea Uno", "Línea dos", "Línea Tres")]
        public void Translation_ShouldConsider_LineBreakCharacters(string locale, string line1, string line2, string line3)
        {
            I18N.Current.Locale = locale;

            const string key = "TextWithLineBreakCharacters";
            var textWithLineBreaks = I18N.Current.Translate(key);
            var textWithLineBreaksOrNull = I18N.Current.TranslateOrNull(key);
            var expected = $"{line1}{Environment.NewLine}{line2}{Environment.NewLine}{line3}";

            Assert.AreEqual(expected, textWithLineBreaks);
            Assert.AreEqual(expected, textWithLineBreaksOrNull);

            I18N.Current = new I18N()
                .SetResourcesFolder("JsonKvpLocales")
                .AddLocaleReader(new JsonKvpReader(), ".json")
                .Init(GetType().Assembly);

            I18N.Current.Locale = locale;

            textWithLineBreaks = I18N.Current.Translate(key);
            textWithLineBreaksOrNull = I18N.Current.TranslateOrNull(key);
            expected = $"{line1}{Environment.NewLine}{line2}{Environment.NewLine}{line3}";

            Assert.AreEqual(expected, textWithLineBreaks);
            Assert.AreEqual(expected, textWithLineBreaksOrNull);

            I18N.Current = new I18N()
                .SetResourcesFolder("JsonListLocales")
                .AddLocaleReader(new JsonListReader(), ".json")
                .Init(GetType().Assembly);

            I18N.Current.Locale = locale;

            textWithLineBreaks = I18N.Current.Translate(key);
            textWithLineBreaksOrNull = I18N.Current.TranslateOrNull(key);
            expected = $"{line1}{Environment.NewLine}{line2}{Environment.NewLine}{line3}";

            Assert.AreEqual(expected, textWithLineBreaks);
            Assert.AreEqual(expected, textWithLineBreaksOrNull);
        }

        [TestCase("en", "Good", "Snake")]
        [TestCase("es", "Serpiente", "Buena")]
        public void EnumTranslation_ShouldConsider_LineBreakCharacters(string locale, string line1, string line2)
        {
            I18N.Current.Locale = locale;
            var animals = I18N.Current.TranslateEnumToDictionary<Animals>();

            Assert.AreEqual($"{line1}{Environment.NewLine}{line2}", animals[Animals.Snake]);
        }

        [TestCase("en", "Line One", "Line Two", "Line Three")]
        [TestCase("es", "Línea Uno", "Línea Dos", "Línea Tres")]
        public void Multiline_IsSupported_OnLocales(string locale, string line1, string line2, string line3)
        {
            I18N.Current.Locale = locale;
            var multilineValue = I18N.Current.Translate("Multiline");
            var expected = $"{line1}{Environment.NewLine}{line2}{Environment.NewLine}{line3}";

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
            Assert.IsNull("nonExistentKey".TranslateOrNull());
        }

        [Test]
        public void SetThrowWhenKeyNotFound_WillThrow_WhenKeyNotFound()
        {
            I18N.Current.SetThrowWhenKeyNotFound(true);

            Assert.Throws<KeyNotFoundException>(() => I18N.Current.Translate("fake"));
            Assert.Throws<KeyNotFoundException>(() => "fake".Translate());
        }

        [Test]
        public void NotFoundSymbol_ShoulNever_BeNullOrEmpty()
        {
            I18N.Current.SetNotFoundSymbol("##");
            I18N.Current.SetNotFoundSymbol(null);

            Assert.AreEqual("##missing##", "missing".Translate());

            I18N.Current.SetNotFoundSymbol(string.Empty);

            Assert.AreEqual("##missing##", "missing".Translate());
        }
        
        [Test]
        public void Namespace_CanBeTranslated()
        {
            // Arrange
            I18N.Current.Locale = "en";
            
            // Act
            var testViewModelTitle = I18N.Current.Translate("TestViewModel.Title");
            var testViewModelDescription = I18N.Current.Translate("TestViewModel.Description");
            var testViewModelAnimals = I18N.Current.TranslateEnumToTupleList<Animals>("TestViewModel");
            
            // Assert
            Assert.AreEqual(testViewModelTitle, "TestViewModel Title");
            Assert.True(testViewModelDescription.Contains("\n"));
            
            Assert.AreEqual(testViewModelAnimals[0].Item2, "TestViewModel Dog");
            Assert.AreEqual(testViewModelAnimals[1].Item2, "TestViewModel Cat");
        }
        
        [Test]
        public void GetSection_CanBeTranslated()
        {
            // Arrange
            I18N.Current.Locale = "en";
            
            // Act
            var section = I18N.Current.GetSection("TestViewModel");
            
            var testViewModelTitle = section.Translate("Title");
            var testViewModelDescription = section.Translate("Description");
            var testViewModelAnimals = section.TranslateEnumToTupleList<Animals>();
            
            // Assert
            Assert.AreEqual(section.Name, "TestViewModel");
            Assert.AreEqual(testViewModelTitle, "TestViewModel Title");
            Assert.True(testViewModelDescription.Contains("\n"));
            
            Assert.AreEqual(testViewModelAnimals[0].Item2, "TestViewModel Dog");
            Assert.AreEqual(testViewModelAnimals[1].Item2, "TestViewModel Cat");
        }
        
        [Test]
        public void GetSection_SetThrowWhenKeyNotFound_WillThrow_WhenKeyNotFound()
        {
            // Arrange
            I18N.Current.Locale = "en";
            I18N.Current.SetThrowWhenKeyNotFound(true);
            var section = I18N.Current.GetSection("TestViewModel");
            
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => section.Translate("fake"));
        }
    }
}
