using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

namespace I18NPortable.UnitTests
{
	[TestFixture()]
	public class I18NTests
	{
		[SetUp]
		public void Init() => 
			I18N.Current = new I18N()
				.SetNotFoundSymbol("?")
				.SetThrowWhenKeyNotFound(false)
				.Init(GetType().GetTypeInfo().Assembly);

	    [TearDown]
	    public void Finish() => 
            I18N.Current.Dispose();

		[Test]
		public void EmbbededLocales_ShouldBe_Discovered()
		{
			var languages = I18N.Current.Languages;
			Assert.AreEqual(2, languages.Count);
		}

		[Test]
		public void Languages_ShouldHave_CorrectDisplayName()
		{
			var languages = I18N.Current.Languages;
			var es = languages.FirstOrDefault(x => x.Locale.Equals("es"));
			var en = languages.FirstOrDefault(x => x.Locale.Equals("en"));

			Assert.AreEqual("English", en?.DisplayName);
			Assert.AreEqual("Español", es?.ToString());
		}

	    [Test]
        public void DiscoverLocales_ShouldThrow_If_NoLocalesAvailable()
	    {
            Assert.Throws<Exception>(() =>
                I18N.Current.Init(I18N.Current.GetType().GetTypeInfo().Assembly));
	    }

	    [Test]
	    public void TryingToReload_CurrentLoadedLanguage_WillDoNothing()
	    {
	        I18N.Current.Locale = "en";
	        I18N.Current.Locale = "es";

            var logs = new List<string>();
	        Action<string> logger = text => logs.Add(text);
	        I18N.Current.SetLogger(logger);
	        I18N.Current.Locale = "es";

            Assert.AreEqual(1, logs.Count);
	    }

		[Test]
		public void CorrectLocale_IsLoaded_WhenSettingLanguage()
		{
			var languages = I18N.Current.Languages;
			var es = languages.FirstOrDefault(x => x.Locale.Equals("es"));
			var en = languages.FirstOrDefault(x => x.Locale.Equals("en"));

			I18N.Current.Language = es;
			Assert.AreEqual("es", I18N.Current.Locale);

			I18N.Current.Language = en;
			Assert.AreEqual("en", I18N.Current.Locale);
		}

		[Test]
		public void CurrentLanguage_Match_LoadedLocale()
		{
			I18N.Current.Locale = "en";
			Assert.AreEqual("en", I18N.Current.Language.Locale);

			I18N.Current.Locale = "es";
			Assert.AreEqual("es", I18N.Current.Language.Locale);
		}

	    [Test]
	    public void LoadLanguage_ShouldLoad_LanguageLocale()
	    {
	        var language = new PortableLanguage { Locale = "en", DisplayName = "English" };
	        I18N.Current.Language = language;

            Assert.AreEqual("one", I18N.Current.Translate("one"));

            language = new PortableLanguage { Locale = "es", DisplayName = "Español" };
            I18N.Current.Language = language;

            Assert.AreEqual("uno", I18N.Current.Translate("one"));
        }

		[Test]
		public void LoadingNonExistentLocale_ShouldThrow()
		{
		    Assert.Throws<KeyNotFoundException>(() => I18N.Current.Locale = "fr");
		}

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
            Assert.AreEqual("Hola Diego, tienes 3 emails", I18N.Current.TranslateOrNull("Mailbox.Notification", "Diego", "3"));
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
	    public void FallbackLocale_ShouldBeLoaded_WhenRequestedLocale_IsNotAvailable()
	    {
            CultureInfo.DefaultThreadCurrentCulture = 
                CultureInfo.DefaultThreadCurrentUICulture =
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
            
            I18N.Current.SetFallbackLocale("en").Init(GetType().GetTypeInfo().Assembly);

            Assert.AreEqual("en", I18N.Current.Locale);
	    }

        [Test]
        public void FallbackLocale_ShouldBeIgnored_IfNotAvailable()
        {
            CultureInfo.DefaultThreadCurrentCulture =
                CultureInfo.DefaultThreadCurrentUICulture =
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");

            I18N.Current.SetFallbackLocale("fr").Init(GetType().GetTypeInfo().Assembly);

            Assert.AreEqual("en", I18N.Current.Locale);
        }

        [Test]
		public void Logger_CanBeSet_AsAction()
		{
			var logs = new List<string>();
			Action<string> logger = text => logs.Add(text);

			I18N.Current.SetLogger(logger);
			I18N.Current.Locale = "en";
			I18N.Current.Locale = "es";

			Assert.IsTrue(logs.Count > 0);
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

	    [Test]
	    public void I18N_CanBeMocked()
	    {
	        var mock = new I18NMock();
	        I18N.Current = mock;

            Assert.AreEqual("mocked translation", I18N.Current.Translate("something"));
	    }

	    [Test]
	    public void I18n_CanBeDisposed()
	    {
            I18N.Current.PropertyChanged += (sender, args) => { };
            I18N.Current.Dispose();

            Assert.IsNull(I18N.Current.Language);
            Assert.IsNull(I18N.Current.Languages);
            Assert.IsNull(I18N.Current.Locale);
	    }

	    [Test]
	    public void Indexer_Is_Bindable()
	    {
	        I18N.Current.Locale = "es";
	        var translation = I18N.Current.Translate("one");

            I18N.Current.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("Item[]"))
                    translation = I18N.Current.Translate("one");
            };

	        I18N.Current.Locale = "en";

            Assert.AreEqual("one", translation);

            I18N.Current.Locale = "es";

            Assert.AreEqual("uno", translation);
        }

        [Test]
        public void Language_Is_Bindable()
        {
            I18N.Current.Locale = "es";
            var language = I18N.Current.Language;

            I18N.Current.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("Language"))
                    language = I18N.Current.Language;
            };

            I18N.Current.Locale = "en";

            Assert.AreEqual("en", language.Locale);

            I18N.Current.Locale = "es";

            Assert.AreEqual("es", language.Locale);
        }

        [Test]
        public void Locale_Is_Bindable()
        {
            I18N.Current.Locale = "es";
            var locale = I18N.Current.Locale;

            I18N.Current.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("Locale"))
                    locale = I18N.Current.Locale;
            };

            I18N.Current.Locale = "en";

            Assert.AreEqual("en", locale);

            I18N.Current.Locale = "es";

            Assert.AreEqual("es", locale);
        }

	    [Test]
	    public void DefaultLocale_Respects_CurentCulture()
	    {
            I18N.Current.Dispose();

            CultureInfo.DefaultThreadCurrentCulture =
                CultureInfo.DefaultThreadCurrentUICulture =
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");

            I18N.Current = new I18N().Init(GetType().GetTypeInfo().Assembly);

            Assert.AreEqual("es", I18N.Current.GetDefaultLocale());
        }
    }
}
