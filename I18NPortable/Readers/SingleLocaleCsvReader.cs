using System;
using System.Collections.Generic;
using System.IO;

namespace I18NPortable.Readers
{
    public class SingleLocaleCsvReader : ILocaleReader
    {
        public Dictionary<string, string> Read(Stream stream)
        {
            var translations = new Dictionary<string, string>();

            using (var streamReader = new StreamReader(stream))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    
                    if(string.IsNullOrWhiteSpace(line))
                        continue;
                    
                    var kvp = line.Split(new []{';'}, 2, StringSplitOptions.RemoveEmptyEntries);
                    
                    if(kvp.Length != 2)
                        continue;

                    var key = kvp[0].Trim();
                    var value = kvp[1].Trim().UnescapeLineBreaks();
                    
                    translations.Add(key, value);
                }
            }

            return translations;
        }
    }
}