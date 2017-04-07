using System;
using System.Linq;
using I18NPortable;
using Sample.Classic.Core;
using UIKit;

namespace Sample.Classic.iOS
{
    public partial class HomeViewController : UIViewController
    {
        public HomeViewController() 
            : base("HomeViewController", null) {}


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UpdateTranslations();

            ChangeLanguageButton.TouchUpInside += ChangeLanguageButtonOnTouchUpInside;
        }

        private void UpdateTranslations()
        {
            LoadedLanguageTitle.Text = "LoadedLanguage.Title".Translate();
            LoadedLanguageValue.Text = I18N.Current.Language.DisplayName;

            AvailableLanguagesTitle.Text = "AvailableLanguages".Translate();
            AvailableLanguagesLabel.Text = string.Join(", ", I18N.Current.Languages.Select(x => x.DisplayName));

            SingleEnumTitle.Text = "SingleEnum.Title".Translate();
            SingleEnumLabel.Text = Animals.Dog.Translate();

            EnumTitle.Text = "EnumValues.Title".Translate();
            EnumLabel.Text = string.Join(", ", I18N.Current.TranslateEnumToList<Animals>());

            GreetingTitle.Text = "Greeting.Title".Translate();
            GreetingValue.Text = "Greeting.Value".Translate("Diego");

            MultilineTitle.Text = "Multiline.Title".Translate();
            MultilineValue.Text = "Multiline.Value".Translate();

            ChangeLanguageButton.SetTitle("ChangeLanguage".Translate(), UIControlState.Normal);
        }

        private void ChangeLanguageButtonOnTouchUpInside(object sender, EventArgs eventArgs)
        {
            var languages = I18N.Current.Languages
                .Where(x => x.Locale != I18N.Current.Locale);

            var actionSheet = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

            foreach (var language in languages)
            {
                actionSheet.AddAction(UIAlertAction.Create(language.DisplayName,
                    UIAlertActionStyle.Default, action => ChangeLanguage(language)));
            }

            actionSheet.AddAction(UIAlertAction.Create("Cancel".Translate(),
                UIAlertActionStyle.Cancel, null));

            PresentViewController(actionSheet, true, null);
        }

        private void ChangeLanguage(PortableLanguage language)
        {
            I18N.Current.Language = language;
            UpdateTranslations();
        }
    }
}