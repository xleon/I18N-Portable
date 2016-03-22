using System.Diagnostics;
using System.Reflection;
using I18NPortable;

namespace SampleApp.Core
{
	public enum Animals
	{
		Dog,
		Cat,
		Rat,
		Tiger,
		Monkey	
	}

	public class App
    {
	    public App()
	    {
			I18N.Current.Init(GetType().GetTypeInfo().Assembly);
		    var languages = I18N.Current.Languages;
		    var one = "one".Translate();

		    var enumTranslation = I18N.Current.TranslateEnum<Animals>();

			Debug.WriteLine(one);
	    }
    }
}
