using System.Diagnostics;
using System.Linq;
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

	public class AnimalListItem
	{
		public Animals Animal { get; set; }
		public string DisplayName { get; set; }
	}

	public class App
    {
	    public App()
	    {
			I18N.Current.Init(GetType().GetTypeInfo().Assembly);
		    var languages = I18N.Current.Languages;
		    var one = "one".Translate();

		    var animalsTranslation = I18N.Current.TranslateEnum<Animals>();
		    var animalList =
			    animalsTranslation.Select(x => new AnimalListItem {Animal = x.Key, DisplayName = x.Value}).ToList();

			Debug.WriteLine(one);
	    }
    }
}
