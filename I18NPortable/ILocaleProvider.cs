using System;
using System.Collections.Generic;
using System.Reflection;

namespace I18NPortable
{
	public interface ILocaleProvider
	{
		Dictionary<string, string> GetAvailableLocales(Assembly hostAssembly);
		Dictionary<string, string> PrepareTranslationForLocale(Assembly hostAssembly, string resourcePath);
	}
}
