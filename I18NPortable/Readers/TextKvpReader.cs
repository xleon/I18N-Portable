using System;
using System.Collections.Generic;
using System.IO;

namespace I18NPortable.Readers
{
    public class TextKvpReader : ILocaleReader
    {
        public Dictionary<string, string> Read(Stream stream)
        {
            var translations = new Dictionary<string, string>();

            using (var streamReader = new StreamReader(stream))
            {
                string key = null;
                string value = null;
                string section = null;

                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var isEmpty = string.IsNullOrWhiteSpace(line);
                    var isComment = !isEmpty && line.Trim().StartsWith("#");
                    var isKeyValuePair = !isEmpty && !isComment && line.Contains("=");

                    // --> end any prefix
                    if (!isEmpty && line.Trim().EndsWith("}"))
                    {
                        if (key != null && value != null)
                        {
                            translations.Add(TryBindSectionToKey(section, key), value);
                        }

                        section = null;
                        key = null;
                        value = null;
                        continue;
                    }

                    if ((isEmpty || isComment || isKeyValuePair) && key != null && value != null)
                    {                   
                        translations.Add(TryBindSectionToKey(section, key), value);
                        
                        key = null;
                        value = null;
                    }

                    if (isEmpty || isComment)
                        continue;

                    if (isKeyValuePair)
                    {
                        var kvp = line.Split(new[] { '=' }, 2);

                        key = kvp[0].Trim();
                        value = kvp[1].Trim().UnescapeLineBreaks();
                        // --> start any prefix
                        if (value.StartsWith("{"))
                        {
                            section = key;
                            key = null;
                            value = null;
                        }
                    }
                    else if (key != null && value != null)
                    {
                        value = value + Environment.NewLine + line.Trim().UnescapeLineBreaks();
                    }
                }

                if (key != null && value != null)
                    translations.Add(TryBindSectionToKey(section, key), value);
            }

            return translations;
        }

        private static string TryBindSectionToKey(string prefix, string key)
        {
            // try to add any prefix
            return string.IsNullOrEmpty(prefix) ? key : $"{prefix}.{key}";
        }
    }
}
