using System.Collections.Generic;
using System.IO;

namespace I18NPortable.Readers
{
    public class JsonKvpReader : ILocaleReader
    {
        public Dictionary<string, string> Read(Stream stream)
        {
            var translations = new Dictionary<string, string>();

            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
            }

            return translations;
        }
    }
}
