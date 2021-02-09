
# I18N-Portable
Simple and cross platform internationalization/translations for Xamarin and .NET

[![NuGet](https://img.shields.io/nuget/v/i18nportable?style=for-the-badge)](https://www.nuget.org/packages/I18NPortable/) 
[![NuGet](https://img.shields.io/nuget/dt/i18nportable?style=for-the-badge)](https://www.nuget.org/packages/I18NPortable/) 
[![AppVeyor](https://img.shields.io/appveyor/ci/xleon/i18n-portable?style=for-the-badge)](https://ci.appveyor.com/project/xleon/i18n-portable) 
[![Codecov](https://img.shields.io/codecov/c/github/xleon/I18N-Portable?style=for-the-badge)](https://codecov.io/gh/xleon/I18N-Portable)

- Cross platform
- Simple to use: `"key".Translate()`.
- Simple and fluent initialization setup.
- Readable locale files (.txt with key/value pairs).
- Support for custom file formats (json, xml, etc)
- Light weight
- No dependencies.
- Well tested

![https://cloud.githubusercontent.com/assets/145087/24824462/c5a0ecce-1c0b-11e7-84d3-4f0fa815c9da.png](https://cloud.githubusercontent.com/assets/145087/24824462/c5a0ecce-1c0b-11e7-84d3-4f0fa815c9da.png)


### Install

Install it on your PCL and platform projects.
From nuget package manager console: 

`PM> Install-Package I18NPortable`

### Setup locales

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

[Other file formats (including custom) supported](https://github.com/xleon/I18N-Portable#custom-formats)

### Fluent initialization

```csharp
I18N.Current
    .SetNotFoundSymbol("$") // Optional: when a key is not found, it will appear as $key$ (defaults to "$")
    .SetFallbackLocale("en") // Optional but recommended: locale to load in case the system locale is not supported
    .SetThrowWhenKeyNotFound(true) // Optional: Throw an exception when keys are not found (recommended only for debugging)
    .SetLogger(text => Debug.WriteLine(text)) // action to output traces
    .SetResourcesFolder("OtherLocales") // Optional: The directory containing the resource files (defaults to "Locales")
    .Init(GetType().GetTypeInfo().Assembly); // assembly where locales live
```

### Usage

```csharp
string one = "one".Translate();
string notification = "Mailbox.Notification".Translate("Diego", 3); // same as string.Format(params). Output: Hello Diego, you've got 3 emails
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

`I18N` implements `INotifyPropertyChanged` and it has an indexer to translate keys. For instance, you could translate a key like:

    string three = I18N.Current["three"]; 

With that said, the easiest way to bind your views to `I18N` translations is to use the built-in indexer 
by creating a proxy object in your ViewModel:

```csharp
public abstract class BaseViewModel
{
    public II18N Strings => I18N.Current;
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


### Xamarin Static Binding
You can use static values instead of binding texts. It's more effective and has better performance.

- Install `I18NPortable.Xamarin` extension to your Xamarin Forms Project.

- Go your XAML page and add following xml namespace:
```xml
xmlns:i18n="clr-namespace:I18NPortable.Xamarin.Xaml.Extensions;assembly=I18NPortable.Xamarin"
```

- Then use it with calling `i18n:Translate`

```xml
<Label Text="{l:Localize HelloWorld}" />
```

Entire page sample:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:d="http://xamarin.com/schemas/2014/forms/design"
             xmlns:i18n="clr-namespace:I18NPortable.Xamarin.Xaml.Extensions;assembly=I18NPortable.Xamarin"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             mc:Ignorable="d"
             x:Class="Sample.MainContentPage">
    <ContentPage.Content>
       <StackLayout>
            <Label Text="{i18n:Translate HelloWorld}"
                VerticalOptions="CenterAndExpand" 
                HorizontalOptions="CenterAndExpand" />
        </StackLayout>
    </ContentPage.Content>
</ContentPage>
```

### Supported formats

The library ships with a single format reader/parser that is [TextKvpReader](https://github.com/xleon/I18N-Portable/blob/master/I18NPortable/Readers/TextKvpReader.cs). Any other reader will be isolated in a different nuget/plugin to keep the library as simple as possible.

| Reader        | Format        | Source  |
| ------------- |:-------------:| :-----:|
| [TextKvpReader](https://github.com/xleon/I18N-Portable/blob/master/I18NPortable/Readers/TextKvpReader.cs)    | [See sample](https://github.com/xleon/I18N-Portable/blob/master/I18NPortable.UnitTests/Locales/es.txt) | I18NPortable |
| [JsonKvpReader](https://github.com/xleon/I18N-Portable/blob/master/I18NPortable.JsonReader/JsonKvpReader.cs)    | [See sample](https://github.com/xleon/I18N-Portable/blob/master/I18NPortable.UnitTests/JsonKvpLocales/es.json) | I18NPortable.JsonReader [![I18NPortable.JsonReader](https://img.shields.io/nuget/v/I18NPortable.JsonReader.svg?maxAge=50000)](https://www.nuget.org/packages/I18NPortable.JsonReader/) |
| [JsonListReader](https://github.com/xleon/I18N-Portable/blob/master/I18NPortable.JsonReader/JsonListReader.cs)   | [See sample](https://github.com/xleon/I18N-Portable/blob/master/I18NPortable.UnitTests/JsonListLocales/es.json) | I18NPortable.JsonReader [![I18NPortable.JsonReader](https://img.shields.io/nuget/v/I18NPortable.JsonReader.svg?maxAge=50000)](https://www.nuget.org/packages/I18NPortable.JsonReader/) |

To use any non-default format, it needs to be added on initialization:

```csharp
I18N.Current
    .AddLocaleReader(new JsonKvpReader(), ".json") // ILocaleReader, file extension
    // add more readers here if you need to
    .Init(GetType().Assembly);
```

### Creating a custom reader for another file format:

It's very easy to create custom readers/parsers for any file format you wish.
For instance, lets take a loot at the above mentioned `JsonKvpReader`:

Given this __en.json__ file
```json
{
  "one": "uno",
  "two": "dos",
  "three": "tres"
}
```

Creating a custom reader is as simple as implementing `ILocaleReader`:

```csharp
public interface ILocaleReader
{
    Dictionary<string, string> Read(Stream stream);
}
```

```csharp
public class JsonKvpReader : ILocaleReader
{
    public Dictionary<string, string> Read(Stream stream)
    {
        using (var streamReader = new StreamReader(stream))
        {
            var json = streamReader.ReadToEnd();

            return JsonConvert
                .DeserializeObject<Dictionary<string, string>>(json)
                .ToDictionary(x => x.Key.Trim(), x => x.Value.Trim().UnescapeLineBreaks());
        }
    }
}
```

### Contributing new readers

If you implemented a new reader for another file format and you want to contribute, feel free to make a pull request. Any new reader will live in their own project in the solution and will produce a different nuget as a plugin to I18NPortable.

    
