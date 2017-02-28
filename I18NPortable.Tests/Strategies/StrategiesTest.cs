using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Collections.Generic;

namespace I18NPortable.Tests.Strategies
{
    [TestClass]
    public class StrategiesTest
    {
        class FailingStrategie : I18NPortable.Strategies.ILocaleReceiveStrategy
        {
            public bool TryGetTranslationDictionary(out Dictionary<string, string> result)
            {
                result = null;
                return false;
            }
        }

        class SusseccesfulStrategie : I18NPortable.Strategies.ILocaleReceiveStrategy
        {
            private string rString;

            public SusseccesfulStrategie(string returnValue)
            {
                rString = returnValue;
            }

            public bool TryGetTranslationDictionary(out Dictionary<string, string> result)
            {
                result = new Dictionary<string, string> { { "Test", rString } };
                return true;
            }
        }

        [TestMethod]
        public void CanGetResultEvenWhenStrategieIsFailed()
        {
            string test = "example";
            var sc = new I18NPortable.Strategies.LocaleStrategieCollection(new FailingStrategie(), new SusseccesfulStrategie(test));
            Assert.AreEqual(test, sc.GetLocaleTranslationDictionary()["Test"]);
        }

        [TestMethod]
        public void StrategiesAreOrdered()
        {
            string test = "example";
            string test2 = "example2";
            var sc = new I18NPortable.Strategies.LocaleStrategieCollection(new SusseccesfulStrategie(test), new SusseccesfulStrategie(test2));
            Assert.AreEqual(test, sc.GetLocaleTranslationDictionary()["Test"]);
        }

    }
}
