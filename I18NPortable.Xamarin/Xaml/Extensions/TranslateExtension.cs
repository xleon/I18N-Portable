using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace I18NPortable.Xamarin.Xaml.Extensions
{
    [ContentProperty("Key")]
    public class TranslateExtension : IMarkupExtension<string>
    {
        public string Key { get; set; }

        public object[] Args { get; set; }

        public string ProvideValue(IServiceProvider serviceProvider)
        {
            return I18N.Current.Translate(Key, Args ?? new object[0]);
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
