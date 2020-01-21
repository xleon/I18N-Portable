using System.Diagnostics;
using System.Reflection;
using I18NPortable;

namespace Sample.XF
{
    public partial class App
    {
        public App ()
        {
            InitializeComponent();

            var currentAssembly = GetType().GetTypeInfo().Assembly;

            I18N.Current
                .SetLogger(text => Debug.WriteLine(text))
                .SetNotFoundSymbol("⛔")
                .SetFallbackLocale("en")
                .Init(currentAssembly);

            MainPage = new Views.MainPage();
        }
    }
}