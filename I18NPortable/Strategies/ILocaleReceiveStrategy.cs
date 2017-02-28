using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I18NPortable.Strategies
{
    public interface ILocaleReceiveStrategy
    {
        bool TryGetTranslationDictionary(out Dictionary<string, string> result);
    }
}