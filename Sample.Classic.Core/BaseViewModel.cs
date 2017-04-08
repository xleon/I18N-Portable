using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using I18NPortable;
using Sample.Classic.Core.Annotations;

namespace Sample.Classic.Core
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public II18N Strings => I18N.Current;
        public string LoadedLanguage => I18N.Current.Language.DisplayName;
        public string AvailableLanguages => string.Join(", ", I18N.Current.Languages.Select(x => x.DisplayName));
        public string Dog => Animals.Dog.Translate();
        public string EnumValues => string.Join(", ", I18N.Current.TranslateEnumToList<Animals>());
        public string GreetingValue => "Greeting.Value".Translate("Windows");

        public IEnumerable<PortableLanguage> LanguagesToSelect 
            => I18N.Current.Languages
                .Where(x => x.Locale != I18N.Current.Locale);

        public void LoadLocale(string locale)
        {
            I18N.Current.Locale = locale;

            OnPropertyChanged(nameof(LoadedLanguage));
            OnPropertyChanged(nameof(AvailableLanguages));
            OnPropertyChanged(nameof(Dog));
            OnPropertyChanged(nameof(EnumValues));
            OnPropertyChanged(nameof(GreetingValue));
        }
    }
}
