using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace I18NPortable.JsonReader
{
    public class JsonKvp
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class JsonListReader : ILocaleReader
    {
        public Dictionary<string, string> Read(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();

                return JsonConvert
                    .DeserializeObject<List<JsonKvp>>(json)
                    .ToDictionary(x => x.Key.Trim(), x => x.Value.Trim().UnescapeLineBreaks());
            }
        }
    }
}
