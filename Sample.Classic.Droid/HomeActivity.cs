using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using I18NPortable;
using Sample.Classic.Core;

namespace Sample.Classic.Droid
{
    [Activity(Label = "I18N-Portable", 
        MainLauncher = true, 
        Icon = "@drawable/icon",
        Theme = "@android:style/Theme.NoTitleBar")]
    public class HomeActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var app = new App();

            SetContentView(Resource.Layout.Home);

            UpdateTranslations();

            FindViewById<Button>(Resource.Id.changeLanguageButton).Click += ChangeLanguageButtonOnTouchUpInside;
        }

        private void UpdateTranslations()
        {
            FindViewById<TextView>(Resource.Id.loadedLanguageTitle).Text = "LoadedLanguage.Title".Translate();
            FindViewById<TextView>(Resource.Id.loadedLanguageValue).Text = I18N.Current.Language.DisplayName;

            FindViewById<TextView>(Resource.Id.availableLanguagesTitle).Text = "AvailableLanguages".Translate();
            FindViewById<TextView>(Resource.Id.availableLanguagesValue).Text = string.Join(", ", I18N.Current.Languages.Select(x => x.DisplayName));

            FindViewById<TextView>(Resource.Id.singleEnumTitle).Text = "SingleEnum.Title".Translate();
            FindViewById<TextView>(Resource.Id.singleEnumValue).Text = Animals.Dog.Translate();

            FindViewById<TextView>(Resource.Id.enumTitle).Text = "EnumValues.Title".Translate();
            FindViewById<TextView>(Resource.Id.enumValue).Text = string.Join(", ", I18N.Current.TranslateEnumToList<Animals>());

            FindViewById<TextView>(Resource.Id.greetingTitle).Text = "Greeting.Title".Translate();
            FindViewById<TextView>(Resource.Id.greetingValue).Text = "Greeting.Value".Translate("Android");

            FindViewById<TextView>(Resource.Id.multilineTitle).Text = "Multiline.Title".Translate();
            FindViewById<TextView>(Resource.Id.multilineValue).Text = "Multiline.Value".Translate();

            FindViewById<Button>(Resource.Id.changeLanguageButton).Text = "ChangeLanguage".Translate();
        }

        private void ChangeLanguageButtonOnTouchUpInside(object sender, EventArgs eventArgs)
        {
            var languages = I18N.Current.Languages
                .Where(x => x.Locale != I18N.Current.Locale)
                .ToList();
            
            var adapter = new ArrayAdapter<PortableLanguage>(this, 
                Android.Resource.Layout.SelectDialogSingleChoice, 
                languages);

            new AlertDialog
                .Builder(this)
                .SetNegativeButton("Cancel".Translate(), (o, args) => {})
                .SetAdapter(adapter, (o, args) =>
                {
                    var language = languages[args.Which];
                    ChangeLanguage(language);
                })
                .Show();
        }

        private void ChangeLanguage(PortableLanguage language)
        {
            I18N.Current.Language = language;
            UpdateTranslations();
        }
    }
}