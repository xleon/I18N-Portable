using System;
using System.Collections.Generic;
using System.Reflection;

namespace I18NPortable
{
	public interface ILocaleProvider
	{
		Dictionary<string, Strategies.LocaleStrategieCollection> GetAvailableLocales();
	}
}
