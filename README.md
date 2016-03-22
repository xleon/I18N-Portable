# I18NPortable
Simple Cross platform internationalization for Xamarin apps

# Features

- No strings duplication across platform projects. Your locale files will live in the "core" (pcl) project
- Simple to use: `var tranlation = "someKey".Translate();`
- Simple editing: just text, no json, no xml. i.e: `home.username.label = User name`
- Auto loads locales based on the system current culture
- Auto list of available languages (bindable)
- Enum translations (for UI pickers, lists, etc)

