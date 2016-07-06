### Install

From nuget package manager console: 

`PM> Install-Package I18NPortable`

You will usually install it on your PCL project. 
If you need to get translations/strings from platform projects, you will need to install it in those as well.

### Setup locales

In your PCL/Core project, create a directory called "Locales".
Create a `{two letter ISO code}.txt` file for each language you want to support 
(Don´t forget to set "Build Action" to "Embedded Resource" on the properties of each file) :

    YourProject.Core
        |-- Locales
            |-- en.txt
            |-- es.txt
            |-- fr.txt
            

**Locale content sample**

    # key = value (the key will be the same across locales)
    one = uno
    two = dos
    three = tres
    four = cuatro
    five = cinco
    
    # Enums are supported
    Animals.Dog = Perro
    Animals.Cat = Gato
    Animals.Rat = Rata
    Animals.Tiger = Tigre
    Animals.Monkey = Mono

    # Support for string.Format()
    stars.count = Tienes {0} estrellas

### Initialization

I18NPortable needs to know which PCL assembly hosts your locale files. Assuming your 
initialization code and your locales live on the Core PCL itself (recommended):

    I18N.Current.Init(GetType().GetTypeInfo().Assembly);

To initialize `I18N` from another project (ie: ios/android/uwp/other PCL):

    var assembly = typeof(AnyClassInYourHostPCL).GetTypeInfo().Assembly;
    I18N.Current.Init(assembly);
    
`I18N` will scan the "Locales" directory to get the available/supported languages. 
Then it will try to load the correct locale matching the system/OS current culture.
In case the current culture is not supported (your system is in French but "fr.txt" does not exists), 
the first locale in the list will be loaded.
Though you can specify which locale will be loaded in case you don´t support it:

    // load en.txt if the system current language is not supported
    I18N.Current.SetFallbackLocale("en").Init(assembly); 

### Show a list of supported languages in your application

Some times you need to show a picker/list with supported languages so the user can change it. 
`I18N` provides a List of `PortableLanguage` objects:

    List<PortableLanguage> languages = I18N.Current.Languages;
    
Each of those have a "Locale" property indicating the ISO code (ie: en, es) and a "DisplayName" property with a human description of the language (ie: English, Spanish)

    public class PortableLanguage
	{
		public string Locale { get; set; }
		public string DisplayName { get; set; }
		public override string ToString() => DisplayName;
	}

### Change language at run time

Option 1: pass a locale ISO code:

    I18N.Current.Locale = "es";

Option 2: pass a `PortableLanguage` instance coming from `I18N.Current.Languages`:

    var language = I18N.Current.Languages[1];
	I18N.Current.Language = language;
    
You can also get the current language/locale:

    var currentLanguage = I18N.Current.Language;
    var currentLocale = I18N.Current.Locale;
    
### Get translations

There are 3 ways to translate a key. Use the one you like, as all of them will produce the same result:

    var one = I18N.Current.Translate("one");
    var two = "two".Translate(); // string extension method
	var three = I18N.Current["three"]; // indexer
	
The first and second methods allow additional arguments 
reproducing `string.Format(params object[] args)` functionality: 

    // in your locale file: Mailbox.Notification = Hello {0}, you´ve got {1} emails
    var notification = "Mailbox.Notification".Translate("Diego", 3);
    var notification2 = I18N.Current.Translate("Mailbox.Notification", "Maria", 5);
    
If the key your are looking for is not present in the current locale, you´ll get the following:

    "?key?"
    
You can change the symbol to show when a key is not found:

    I18N.Current.SetNotFoundSymbol("$$"); 
    
### Data binding and MvvM frameworks
    
The easiest way to bind your views to `I18N` translations is to use the built-in indexer by creating a proxy property in your ViewModel:

    public abstract class BaseViewModel
	{
		public I18N Strings => I18N.Current;
        ...
    }
    
 You will get live updates when loading another locale at run time because `I18N.Current` implements `INotifyPropertyChanged` and references to the indexer will be updated.

**Xaml sample**

    <Button Content="{Binding Strings[key]}" />
    
**Xamarin.Forms sample**

    <Button Text="{Binding Strings[key]}" />`
    
**Android/MvvmCross sample**

    <TextView local:MvxBind="Text Strings[key]" />
                
**iOS/MvvmCross sample**

    var set = this.CreateBindingSet<YourView, YourViewModel>();
    set.Bind(anyUIText).To("Strings[key]");

### Misc

**Enable logger**

    Action<string> logger = text => Debug.WriteLine(text);
    I18N.Current.SetLogger(logger);
    
**Throw an exception whenever a key is not found**

If you prefer to get exceptions rather than not found symbols (like "?"):

    I18N.Current.SetThrowWhenKeyNotFound(true);
    
**TranslateOrNull**

If you want to get null when a key is not found:

    var fake = "anyKey".TranslateOrNull();
    var fake2 = I18N.Current.TranslateOrNull("anyKey");

**Fluent initialization**

    I18N.Current
        .SetLogEnabled(false)
        .SetThrowWhenKeyNotFound(true)
        .SetNotFoundSymbol("$$")
        .SetFallbackLocale("en")
		.SetLogger(text => Debug.WriteLine(text))
        .Init(GetType().GetTypeInfo().Assembly);
        
**Translate Enum**

Given this enum:

    public enum Animals
	{
		Dog,
		Cat,
		Rat,
		Tiger,
		Monkey
	}
    
and these lines in your locale text file:

    Animals.Dog = Perro
    Animals.Cat = Gato
    Animals.Rat = Rata
    Animals.Tiger = Tigre
    Animals.Monkey = Mono
    
You can get a `Dictionary<T, string>` where T is the enum value:

    var animals = I18N.Current.TranslateEnum<Animals>();
	var monkey = animals[Animals.Monkey]; // Mono

### TODO

- Sample projects