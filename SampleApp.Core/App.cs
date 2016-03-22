using System.Diagnostics;
using System.Reflection;
using I18NPortable;

namespace SampleApp.Core
{
	public class App
    {
	    public App()
	    {
			I18N.Current.Init(GetType().GetTypeInfo().Assembly);
		    var languages = I18N.Current.Languages;
		    var one = "one".Translate();

			Debug.WriteLine(one);
	    }
    }
}
