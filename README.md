
# I18N-Portable
Simple and cross platform internationalization/translations for Xamarin and .NET

[![Build status](https://ci.appveyor.com/api/projects/status/ogaeiar24scm9c8e?svg=true)](https://ci.appveyor.com/project/xleon/i18n-portable)
[![codecov.io](https://codecov.io/gh/xleon/I18N-Portable/coverage.svg?branch=master)](https://codecov.io/gh/xleon/I18N-Portable)
[![I18NPortable](https://img.shields.io/nuget/v/I18NPortable.svg?maxAge=92000)](https://www.nuget.org/packages/I18NPortable/)


### Why I18NPortable?
---
- Cross platform
- Simple to use: `"key".Translate()`.
- Simple and fluent setup.
- Readable locale files (.txt with key/value pairs) rather than json or xml.
- Very tiny: about 10kb.
- No dependencies.
- Well tested

![https://cloud.githubusercontent.com/assets/145087/24824462/c5a0ecce-1c0b-11e7-84d3-4f0fa815c9da.png](https://cloud.githubusercontent.com/assets/145087/24824462/c5a0ecce-1c0b-11e7-84d3-4f0fa815c9da.png)
![https://cloud.githubusercontent.com/assets/145087/24824461/c5a04a94-1c0b-11e7-9a30-f14c656e5562.png](https://cloud.githubusercontent.com/assets/145087/24824461/c5a04a94-1c0b-11e7-9a30-f14c656e5562.png)

### Install
---
Install it on your PCL and platform projects.
From nuget package manager console: 

`PM> Install-Package I18NPortable`

### Setup locales
---
- In your PCL/Core project, create a directory called "Locales".
- Create a `{languageCode}.txt` file for each language you want to support. `languageCode` can be a two letter ISO code or a culture name like "en-US". See [full list here](https://msdn.microsoft.com/en-us/library/ee825488%28v=cs.20%29.aspx).
- Set "Build Action" to "Embedded Resource" on the properties of each file         

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


### Fluent initialization
---
```csharp
I18N.Current
    .SetNotFoundSymbol("$") // Optional: when a key is not found, it will appear as $key$ (defaults to "$")
    .SetFallbackLocale("en") // Optional but recommended: locale to load in case the system locale is not supported
    .SetThrowWhenKeyNotFound(true) // Optional: Throw an exception when keys are not found (recommended only for debugging)
    .SetLogger(text => Debug.WriteLine(text)) // action to output traces
    .Init(GetType().GetTypeInfo().Assembly); // assembly where locales live
```     

### Usage
---
```csharp
string one = "one".Translate();
string notification = "Mailbox.Notification".Translate("Diego", 3); // same as string.Format(params). Output: Hello Diego, you´ve got 3 emails
string missingKey = "missing".Translate(); // if the key is not found the output will be $key$. Output: $missing$
string giveMeNull = "missing".TranslateOrNull(); // Output: null

string dog = Animals.Dog.Translate(); // translate enum value (Animals is an Enum backed up in the locale file with "Animals.Dog = Perro")

List<string> animals = I18N.Current.TranslateEnumToList<Animals>(); 

List<Tuple<Animals, string>> animals = I18N.Current.TranslateEnumToTupleList<Animals>();
string dog = animals[0].Item2; // Perro

Dictionary<Animals, string> animals = I18N.Current.TranslateEnumToDictionary<Animals>();
string dog = animals[Animals.Dog]; // Perro

// List of supported languages (present in the "Locales" folder) in case you need to show a picker list
List<PortableLanguage> languages = I18N.Current.Languages; // Each `PortableLanguage` has 2 strings: Locale and DisplayName

// change language on runtime
I18N.Current.Language = language; // instance of PortableLanguage

// change language on runtime (option 2)
I18N.Current.Locale = "fr";
```	

### Data binding
---
`I18N` implements `INotifyPropertyChanged` and it has an indexer to translate keys. For instance, you could translate a key like:

    string three = I18N.Current["three"]; 

With that said, the easiest way to bind your views to `I18N` translations is to use the built-in indexer 
by creating a proxy object in your ViewModel:

```csharp
public abstract class BaseViewModel
{
    public I18N Strings => I18N.Current;
}
```

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

### Advanced usage
---
In case you use any DI or Service Locator pattern, `I18N` implements [II18N interface](https://github.com/xleon/I18N-Portable/blob/master/I18NPortable/II18n.cs).
`II18N` can be easily mocked for unit testing purposes. 

```csharp
public class I18NMock : II18N { ... }

var mock = new I18NMock();
var translation = mock.Translate("key");
```
    
