[![Build status](https://ci.appveyor.com/api/projects/status/ogaeiar24scm9c8e?svg=true)](https://ci.appveyor.com/project/xleon/i18n-portable)
[![I18NPortable](https://img.shields.io/nuget/v/I18NPortable.svg?maxAge=2592000)](https://www.nuget.org/packages/I18NPortable/)

### Why I18NPortable?

- Share translations across platforms (iOS, Android, UWP, etc) from a PCL project.
- Use it everywhere: simple projects, cross platform Mvvm frameworks, etc.
- Really simple to setup (less than 5 mins to get up and running).
- Readable locale files (.txt with key/value pairs) rather than json or xml.
- Simple to use: `"key".Translate()`.
- Very tiny: less than 10kb.
- No dependencies.
- Pure PCL, no platform code
- Unit Tested with 100% code coverage.


### Install

Install it on your PCL and platform projects.
From nuget package manager console: 

`PM> Install-Package I18NPortable`

### Setup locales

In your PCL/Core project, create a directory called "Locales".
Create a `{two letter ISO code}.txt` file for each language you want to support 
(Don´t forget to set "Build Action" to "Embedded Resource" on the properties of each file) :

    YourProjectPCL.Core
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
     
    TextWithLineBreakCharacters = Line One\nLine Two\r\nLine Three
     
    Multiline = Line One
        Line Two
        Line Three


### Initialization

I18NPortable needs to know which PCL assembly hosts your locale files. Assuming your 
initialization code and your locales live on the Core PCL itself (recommended):

```csharp
I18N.Current.Init(GetType().GetTypeInfo().Assembly);
```
    
`I18N` will scan the "Locales" directory to get the available/supported languages. 
Then it will try to load the correct locale matching the system/OS current culture.
In case the current culture is not supported (your system is in French but "fr.txt" does not exists), 
the first locale in the list will be loaded.
Though you can specify which locale will be loaded in case you don´t support it:

```csharp
// load en.txt if the system current language is not supported
I18N.Current.SetFallbackLocale("en").Init(assembly); 
````

### Get translations

There are 3 ways to translate a key. Use the one you like, as all of them will produce the same result:
```csharp
string one = I18N.Current.Translate("one");
string two = "two".Translate(); // string extension method
string three = I18N.Current["three"]; // indexer
```	
The first and second methods allow additional arguments 
reproducing `string.Format(params object[] args)` functionality: 
```csharp
// in your locale file: Mailbox.Notification = Hello {0}, you´ve got {1} emails
string notification = "Mailbox.Notification".Translate("Diego", 3);
string notification2 = I18N.Current.Translate("Mailbox.Notification", "Maria", 5);
```

If the key your are looking for is not present in the current locale, you´ll get the following:

```csharp
var missing = "missing".Translate(); // ?missing?
```
You can change the symbol to show when a key is not found:

```csharp
I18N.Current.SetNotFoundSymbol("$$"); 
var missing = "missing".Translate(); // $$missing$$
```
    
### Data binding and MvvM frameworks
    
The easiest way to bind your views to `I18N` translations is to use the built-in indexer 
by creating a proxy property in your ViewModel:

```csharp
public abstract class BaseViewModel
{
    public I18N Strings => I18N.Current;
    ...
}
```

You will get live updates when loading another locale at run time because `I18N.Current` 
implements `INotifyPropertyChanged` and references to the indexer will be updated.

**Xaml sample**
```xaml
<Button Content="{Binding Strings[key]}" />
```
**Xamarin.Forms sample**
```xaml
<Button Text="{Binding Strings[key]}" />`
```    
**Android/MvvmCross sample**
```xml
<TextView local:MvxBind="Text Strings[key]" />
```                
**iOS/MvvmCross sample**

```csharp
var set = this.CreateBindingSet<YourView, YourViewModel>();
set.Bind(anyUIText).To("Strings[key]");
```
### Show a list of supported languages in your application

Some times you need to show a picker/list with supported languages so the user can change it. 

`I18N` provides a List of `PortableLanguage` objects:
```csharp
List<PortableLanguage> languages = I18N.Current.Languages;
```
Each of those have a "Locale" property indicating the ISO code (ie: en, es) and a "DisplayName" 
property with a human translated description of the language (ie: English, Español)

```csharp
public class PortableLanguage
{
    public string Locale { get; set; }
    public string DisplayName { get; set; }
    public override string ToString() => DisplayName;
}
```

### Change language at run time

Option 1: pass a locale ISO code:
```csharp
I18N.Current.Locale = "es";
```
Option 2: pass a `PortableLanguage` instance coming from `I18N.Current.Languages`:
```csharp
PortableLanguage language = I18N.Current.Languages[1];
I18N.Current.Language = language;
```    
You can also get the current language/locale:
```csharp
PortableLanguage currentLanguage = I18N.Current.Language;
string currentLocale = I18N.Current.Locale;
```    

### Misc

**Enable logger**

```csharp
Action<string> logger = text => Debug.WriteLine(text);
I18N.Current.SetLogger(logger);
```    
**Throw an exception whenever a key is not found**

If you prefer to get exceptions rather than not found symbols (like "?"):
```csharp
I18N.Current.SetThrowWhenKeyNotFound(true);
```    

**Fluent initialization**

```csharp
I18N.Current
    .SetThrowWhenKeyNotFound(true)
    .SetNotFoundSymbol("$$")
    .SetFallbackLocale("en")
    .SetLogger(text => Debug.WriteLine(text))
    .Init(GetType().GetTypeInfo().Assembly); // call `Init()` at the end
```     

### Translation helpers

**TranslateOrNull**

If you want to get null when a key is not found:
```csharp
string fake = "anyKey".TranslateOrNull();
string fake2 = I18N.Current.TranslateOrNull("anyKey");
```
   
**Translate Enums**

Given this enum:

```csharp
public enum Animals
{
    Dog,
    Cat,
    Rat
}
```    
and these lines in your locale text file

    Animals.Dog = Perro
    Animals.Cat = Gato
    Animals.Rat = Rata  

there are multiple choices to get the translated values:

```csharp
// Direct method
var dog = Animals.Dog.Translate(); // Perro
var cat = Animals.Cat.Translate(); // Gato
var rat = Animals.Rat.Translate(); // Rata
```

```csharp
// Probably the most useful for picker lists
List<Tuple<Animals, string>> animals = I18N.Current.TranslateEnumToTupleList<Animals>();
string dog = animals[0].Item2; // Perro
// animals[0].Item1 equals the Animals.Dog value
```

```csharp
List<string> animals = I18N.Current.TranslateEnumToList<Animals>();
string dog = animals[0]; // Perro
```

```csharp
Dictionary<Animals, string> animals = I18N.Current.TranslateEnumToDictionary<Animals>();
string dog = animals[Animals.Dog]; // Perro
```

**Translate Types and instances**

Given these translations:

> RecipeDetailScreen = The recipe detail
> Recipe = A fun recipe
> I18NPortable.Tests.WorkoutScreen = Workout
> I18NPortable.Tests.WorkoutRecord = Workout Detail

```csharp
string detailScren = I18N.Current.Translate<RecipeDetailScreen>(); // The recipe detail
string recipe = I18N.Current.Translate<Recipe>(); // A fun recipe

var detailScrenInstance = new RecipeDetailScreen();
string detailScreen = detailScrenInstance.Translate(); // The recipe detail

using I18NPortable.Tests;

string workout = I18N.Current.Translate<WorkoutScreen>(); // Workout
string workoutRecord = I18N.Current.Translate<WorkoutRecord>(); // Workout Detail

var workoutInstance = new WorkoutScreen();
string workout = workoutInstance.Translate(); // Workout
```