using NUnit.Framework;

namespace I18NPortable.UnitTests
{
    [TestFixture]
    public class BindingTests : BaseTest
    {
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
        public void LanguageProperty_Is_Bindable()
        {
            I18N.Current.Locale = "es";
            var language = I18N.Current.Language;

            I18N.Current.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("Language"))
                    language = ((II18N)sender).Language;
            };

            I18N.Current.Locale = "en";

            Assert.AreEqual("en", language.Locale);

            I18N.Current.Locale = "es";

            Assert.AreEqual("es", language.Locale);
        }

        [Test]
        public void LocaleProperty_Is_Bindable()
        {
            I18N.Current.Locale = "es";
            var locale = I18N.Current.Locale;

            I18N.Current.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("Locale"))
                    locale = ((II18N)sender).Locale;
            };

            I18N.Current.Locale = "en";

            Assert.AreEqual("en", locale);

            I18N.Current.Locale = "es";

            Assert.AreEqual("es", locale);
        }
    }
}
