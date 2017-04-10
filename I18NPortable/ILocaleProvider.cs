using System;
using System.Collections.Generic;
using System.IO;

namespace I18NPortable
{
    public interface ILocaleProvider
    {
        Action<string> Logger { set; }
        IEnumerable<string> AvailableLocales { get; }
        Stream GetLocaleStream(string locale);
    }
}