using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace I18NPortable.JsonReader
{
    public class JsonListSingleFileReader : ISingleFileLocaleReader
    {
        public Dictionary<string, string> Read(Stream stream, string locale)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();

                var localeDictionary = JsonConvert
                    .DeserializeObject<Dictionary<string, List<JsonKvp>>> (json)
                    .ToDictionary(x => x.Key.Trim(), x => x.Value);

                if (!localeDictionary.ContainsKey(locale))
                {
                    throw new I18NException("Provided locale " +
                                            $"'{locale}' was not found in resource file");
                }

                return localeDictionary[locale].ToDictionary(x => x.Key.Trim(),
                    x => x.Value.Trim().UnescapeLineBreaks());
            }
        }
    }
}
