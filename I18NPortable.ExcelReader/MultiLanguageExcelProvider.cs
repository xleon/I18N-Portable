using System;
using System.Collections.Generic;
using System.IO;
using I18NPortable.Contracts;

namespace I18NPortable.ExcelReader
{
    public class MultiLanguageExcelProvider : ILocaleProvider
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<string, string>> GetAvailableLocales()
        {
            throw new NotImplementedException();
        }

        public Stream GetLocaleStream(string locale)
        {
            throw new NotImplementedException();
        }

        public ILocaleProvider SetLogger(Action<string> logger)
        {
            throw new NotImplementedException();
        }

        public ILocaleProvider Init()
        {
            throw new NotImplementedException();
        }
    }
}
