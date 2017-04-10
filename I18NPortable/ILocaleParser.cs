using System.Collections.Generic;
using System.IO;

namespace I18NPortable
{
    internal interface ILocaleParser
    {
        void Parse(Stream stream, Dictionary<string, string> translations);
    }
}