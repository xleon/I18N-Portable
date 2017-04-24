using System.Collections.Generic;
using System.IO;

namespace I18NPortable
{
    public interface ILocaleReader
    {
        Dictionary<string, string> Read(Stream stream);
    }
}