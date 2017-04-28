using System.Collections.Generic;
using System.IO;

namespace I18NPortable.Contracts
{
    public interface ILocaleReader
    {
        Dictionary<string, string> Read(Stream stream);
    }
}