## I18NPortable
Simple Cross platform internationalization for Xamarin apps

### Features

- Forget string duplication across platform projects. Your locale files will live in a PCL of your choice
- Simple to use: `var tranlation = "key".Translate();`
- Simple editing: just text. No json, no xml. i.e: `home.username.label = User name`
- A single file for all translations
- Default locale will be loaded acording to the system user language (unless you explicitly set one)
- Bindable list of available languages (for UI language pickers)
- Enum translations (for UI pickers, lists, etc)
- No external dependencies

### Install

Nuget: // todo

### FAQ

**What´s wrong with Resx files?**

Nothing. If you like Resx format go for it :)

**Can I bind translations in my view/view models?**

Yes

**Can I place locales in non PCL projects?**

Yes, but what´s the point? This library is intended to share locales/translations across different projects. 
Otherwise it´s better to use the built-in localization systems

**Can I use this with Mvvmcross?**

Yes. Read below for more info

**Can I separate translations in different files?**

No. The goal of this library is to keep things simple. If you need such funcionality, there are alternatives out there.


