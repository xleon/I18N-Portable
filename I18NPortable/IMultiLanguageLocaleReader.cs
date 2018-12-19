using System.Collections.Generic;
using System.IO;

namespace I18NPortable
{
    public interface IMultiLanguageLocaleReader
    {
        Dictionary<string, string> Read(Stream stream, string locale);
    }
}
