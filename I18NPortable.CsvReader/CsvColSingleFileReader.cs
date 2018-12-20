using System;
using System.Collections.Generic;
using System.IO;

namespace I18NPortable.CsvReader
{
    public class CsvColSingleFileReader : ISingleFileLocaleReader
    {
        public Dictionary<string, string> Read(Stream stream, string locale)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var langDictionary = new Dictionary<string, string>();

                var headerLine = streamReader.ReadLine();

                if (string.IsNullOrEmpty(headerLine) || string.IsNullOrWhiteSpace(headerLine))
                {
                    throw new I18NException("CSV file does not have locale column header");
                }

                var localeHeaderStrIndex = headerLine.IndexOf(locale, StringComparison.Ordinal);
                if (localeHeaderStrIndex == -1)
                {
                    throw new I18NException("Provided locale +" +
                                            $"'{locale}' was not found in file header");
                }

                var headerSplitTab = headerLine.Split(';');
                var localeCol = 0;
                for (var index = 0; index < headerSplitTab.Length; ++index)
                {
                    if (!headerSplitTab[index].Equals(locale)) continue;
                    localeCol = index;
                    break;
                }

                if (localeCol == 0)
                {
                    throw new I18NException("Provided locale +" +
                                            $"'{locale}' was found in first column, first column is reserved for key");
                }

                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    var lineSplit = line.Split(';');
                    var key = lineSplit[0];
                    var value = lineSplit[localeCol];

                    value = value.Replace("\\n", "\n");
                    value = value.Replace("\\r", "\r");

                    if (value.IndexOf("\n", StringComparison.Ordinal) != -1 &&
                        value.IndexOf("\r\n", StringComparison.Ordinal) == -1)
                    {
                        value = value.Replace("\n", "\r\n");
                    }

                    langDictionary.Add(key, value);
                }

                return langDictionary;
            }
        }
    }
}
