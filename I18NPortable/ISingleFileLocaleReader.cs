using System.Collections.Generic;
using System.IO;

namespace I18NPortable
{
    public interface ISingleFileLocaleReader
    {
        Dictionary<string, string> Read(Stream stream, string locale);
    }
}
