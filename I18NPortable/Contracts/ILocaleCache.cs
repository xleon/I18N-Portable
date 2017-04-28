using System;
using System.Collections.Generic;
using System.IO;

namespace I18NPortable.Contracts
{
    public interface ILocaleCache
    {
        event Action<string> OnLocaleUpdated;
        void Save(string localeName, string fileExtension, Stream stream);
        Dictionary<string, string> Get(string localeName, string fileExtension);
    }
}