using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Moq;

namespace TranslatorCore.Tests
{
    public static class Helpers
    {
        public static Mock<ILocaleLoader> GetLoaderMock()
        {
            var mock = new Mock<ILocaleLoader>();
            
            mock
                .Setup(loader => loader.Load("es", null))
                .Returns(new Dictionary<string, string>
                {
                    {"one", "uno"}
                });
            
            mock
                .Setup(loader => loader.Load("es", "screen1"))
                .Returns(new Dictionary<string, string>
                {
                    {"two", "dos"}
                });
            
            mock
                .Setup(loader => loader.Load("en", null))
                .Returns(new Dictionary<string, string>
                {
                    {"one", "one"}
                });
            
            mock
                .Setup(loader => loader.Load("en", "screen1"))
                .Returns(new Dictionary<string, string>
                {
                    {"two", "two"}
                });

            return mock;
        }
        
        public static void SetCulture(string cultureName)
        {
            CultureInfo.DefaultThreadCurrentCulture =
                CultureInfo.DefaultThreadCurrentUICulture =
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);
        }
    }
}