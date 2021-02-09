using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace I18NPortable.Xamarin.Xaml.Extensions
{
    [ContentProperty("Key")]
    public class TranslateOrNullExtension : IMarkupExtension<string>
    {
        public string Key { get; set; }

        public object[] Args { get; set; }


        public string ProvideValue(IServiceProvider serviceProvider)
        {
            return I18N.Current.TranslateOrNull(Key, Args ?? new object[0]);
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
