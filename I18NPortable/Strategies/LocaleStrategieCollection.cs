using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I18NPortable.Strategies
{
    public class LocaleStrategieCollection
    {
        ILocaleReceiveStrategy[] _startegies;

        public LocaleStrategieCollection(params ILocaleReceiveStrategy[] strategies)
        {
            _startegies = strategies;
        }

        public Dictionary<string, string> GetLocaleTranslationDictionary()
        {
            Dictionary<string, string> result = null;

            foreach (var item in _startegies)
            {
                if(item.TryGetTranslationDictionary(out result))
                {
                    break;
                }
            }

            return result;
        }
    }
}
