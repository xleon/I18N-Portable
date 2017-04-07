using System.Diagnostics;
using System.Reflection;
using I18NPortable;

namespace Sample.Classic.Core
{
    public class App
    {
        public App()
        {
            var currentAssembly = GetType().GetTypeInfo().Assembly;

            I18N
                .Current
                .SetLogger(text => Debug.WriteLine(text))
                .SetNotFoundSymbol("⛔")
                .SetFallbackLocale("en")
                .Init(currentAssembly);
        }
    }
}