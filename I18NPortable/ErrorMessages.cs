namespace I18NPortable
{
    public static class ErrorMessages
    {
        public const string ReaderNull = "Locale reader cannot be null";
        public const string ReaderExtensionNeeded = "Locale reader extension is needed";
        public const string ReaderExtensionStartWithDot = "Locale reader extension should start with '.'";
        public const string ReaderExtensionOneChar = "Locale reader extension should contain at least one char";
        public const string ReaderExtensionJustOneDot = "Locale reader extension should contain just one dot";
        public const string ReaderExtensionTwice = "The same extension cannot be added at two different readers";
        public const string ReaderTwice = "The same reader cannot be added twice";
        public const string NoLocalesFound = "No locales found in specified the host assembly";
        public const string ReaderException = "A reader failed to read the file stream";
    }
}
