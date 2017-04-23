using System;
using System.Collections.Generic;
using System.IO;

namespace I18NPortable
{
    public interface ILocaleProvider : IDisposable
    {
        IEnumerable<string> GetAvailableLocales();
        Stream GetLocaleStream(string locale);
        ILocaleProvider SetLogger(Action<string> logger);
        ILocaleProvider Init();
    }
}