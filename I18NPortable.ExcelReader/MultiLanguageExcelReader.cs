using System;
using System.Collections.Generic;
using System.IO;
using I18NPortable.Contracts;

namespace I18NPortable.ExcelReader
{
    public class MultiLanguageExcelReader : ILocaleReader
    {
        public Dictionary<string, string> Read(Stream stream, string locale = null)
        {
            throw new NotImplementedException();
        }
    }
}
