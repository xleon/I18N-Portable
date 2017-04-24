using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace I18NPortable.Readers
{
    public class JsonKvpReader : ILocaleReader
    {
        public Dictionary<string, string> Read(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();

                return JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(json)
                    .ToDictionary(x => x.Key.Trim(), x => x.Value.Trim().UnescapeLineBreaks());
            }
        }
    }
}
