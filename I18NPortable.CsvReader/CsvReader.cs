using System;
using System.Collections.Generic;
using System.IO;

namespace I18NPortable.CsvReader
{
    public class CsvReader : ILocaleReader
    {
        public Dictionary<string, string> Read(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var langDictionary = new Dictionary<string, string>();
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    var semicolonIndex = line.IndexOf(";", StringComparison.Ordinal);
                    var keyStr = line.Substring(0, semicolonIndex);
                    var valueStr = line.Substring(semicolonIndex + 1, line.Length - 1);
                    langDictionary.Add(keyStr, valueStr);
                }

                return langDictionary;
            }
        }
    }
}
