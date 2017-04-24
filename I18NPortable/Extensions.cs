using System;
using System.Reflection;

namespace I18NPortable
{
    public static class Extensions
    {
        /// <summary>
        /// Get a translation from a key, formatting the string with the given params, if any
        /// </summary>
        public static string Translate(this string key, params object[] args)
            => I18N.Current.Translate(key, args);

        /// <summary>
        /// Get a translation from a key, formatting the string with the given params, if any. 
        /// It will return null when the translation is not found
        /// </summary>
        public static string TranslateOrNull(this string key, params object[] args)
            => I18N.Current.TranslateOrNull(key, args);

        public static string CapitalizeFirstCharacter(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.Length == 1)
                return s.ToUpper();

            return s.Remove(1).ToUpper() + s.Substring(1);
        }

        public static string UnescapeLineBreaks(this string str)
            => str
                .Replace("\\r\\n", "\\n")
                .Replace("\\n", Environment.NewLine)
                .Replace("\r\n", "\n")
                .Replace("\n", Environment.NewLine);

        /// <summary>
        /// Translates an Enum value.
        /// 
        /// i.e: <code>var dog = Animals.Dog.Translate()</code> will give "perro" if the the locale
        /// text file contains a line with "Animal.Dog = perro"
        /// </summary>
        public static string Translate(this Enum value)
        {
            var fieldInfo = value.GetType().GetRuntimeField(value.ToString());
            var fieldName = fieldInfo.FieldType.Name;

            return $"{fieldName}.{value}".Translate();
        }
    }
}