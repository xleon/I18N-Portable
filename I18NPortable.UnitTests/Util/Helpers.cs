using System.Globalization;
using System.Threading;

namespace I18NPortable.UnitTests.Util
{
    public class Helpers
    {
        public static void SetCulture(string cultureName)
        {
            CultureInfo.DefaultThreadCurrentCulture =
                CultureInfo.DefaultThreadCurrentUICulture =
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);
        }
    }
}
